﻿using System.Text;

namespace Proxy.Crypto;

public class Rc4Cipher {
    private const int StateLength = 256;

    private byte[] _engineState;
    private byte[] _workingKey;
    private int _x;
    private int _y;

    public Rc4Cipher(byte[] key) {
        _workingKey = key;
        SetKey(_workingKey);
    }

    public Rc4Cipher(string hexString) {
        _workingKey = HexStringToBytes(hexString);
        SetKey(_workingKey);
    }

    private void SetKey(byte[] keyBytes) {
        _workingKey = keyBytes;
        _x = _y = 0;

        _engineState ??= new byte[StateLength];

        for (var i = 0; i < StateLength; i++) _engineState[i] = (byte) i;

        int i1 = 0, i2 = 0;

        for (var i = 0; i < StateLength; i++) {
            i2 = ((keyBytes[i1] & 0xff) + _engineState[i] + i2) & 0xff;

            (_engineState[i], _engineState[i2]) = (_engineState[i2], _engineState[i]);

            i1 = (i1 + 1) % keyBytes.Length;
        }
    }

    private void ProcessBytes(IReadOnlyList<byte> input, int inOff, int length, IList<byte> output, int outOff) {
        for (var i = 0; i < length; i++) {
            _x = (_x + 1) & 0xff;
            _y = (_engineState[_x] + _y) & 0xff;

            (_engineState[_x], _engineState[_y]) = (_engineState[_y], _engineState[_x]);

            output[i + outOff] = (byte) (input[i + inOff] ^ _engineState[(_engineState[_x] + _engineState[_y]) & 0xff]);
        }
    }

    public void Cipher(byte[] packet) {
        ProcessBytes(packet, 5, packet.Length - 5, packet, 5);
    }

    public void Reset() {
        SetKey(_workingKey);
    }

    private static byte[] HexStringToBytes(string key) {
        if (key.Length % 2 != 0) throw new ArgumentException("Invalid hex string!");

        var bytes = new byte[key.Length / 2];
        var c = key.ToCharArray();
        for (var i = 0; i < c.Length; i += 2) {
            var sb = new StringBuilder(2).Append(c[i]).Append(c[i + 1]);
            var j = Convert.ToInt32(sb.ToString(), 16);
            bytes[i / 2] = (byte) j;
        }

        return bytes;
    }
}