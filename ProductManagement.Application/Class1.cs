﻿using System;
using System.Security.Cryptography;

var key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
Console.WriteLine(key);