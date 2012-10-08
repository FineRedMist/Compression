/*	Copyright 2012 Brent Scriver

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at

		http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/

using OneOddSock.IO;

namespace OneOddSock.Compression.Arithmetic.Tests
{
    public static class BitReaderExtension
    {
        public static bool ConditionalRead(this BitStreamReader reader)
        {
            if (reader.BitLength - reader.BitPosition > 0)
                return reader.ReadBoolean();
            return false;
        }
    }
}