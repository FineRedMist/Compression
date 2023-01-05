Libraries for handling Endian conversion, serialization, and compression.

Note: C# libraries now require the following tools installed to compile:
* Portable Library Tools: http://msdn.microsoft.com/en-us/library/gg597391.aspx
* Code Contracts: http://research.microsoft.com/en-us/projects/contracts/
* .NET Framework 4.03 Targeting Pack: http://msdn.microsoft.com/en-US/hh487282.aspx

The following are optional but useful:
* Portable Library Tools Extension: http://visualstudiogallery.msdn.microsoft.com/b0e0b5e9-e138-410b-ad10-00cb3caf4981
* $ Resharper: http://www.jetbrains.com/resharper/
* License Header Manager: http://licensemanager.codeplex.com/
* Git Source Control Provider: http://visualstudiogallery.msdn.microsoft.com/63a7e40d-4d71-4fbb-a23b-d262124b8f4c

The Dynamic Huffman code has a ToDot method to generate graphs, such as: 

```dot
digraph ChangeWeight {
	graph [ranksep=0];
	node [shape=record];
	Node0 [label="{{0|16}}"];	Node0 -> Node1	Node0 -> Node2
	Node1 [label="{{1|10}}"];	Node1 -> Node3	Node1 -> Node4
	Node2 [label="{{2|*NYT*|6}|1}}"];
	Node3 [label="{{3|6}}"];	Node3 -> Node5	Node3 -> Node6
	Node4 [label="{{4|4}}"];	Node4 -> Node7	Node4 -> Node8
	Node5 [label="{{5|a|3}|000}}"];
	Node6 [label="{{6|3}}"];	Node6 -> Node9	Node6 -> Node10
	Node7 [label="{{7|2}}"];	Node7 -> Node11	Node7 -> Node12
	Node8 [label="{{8|2}}"];	Node8 -> Node13	Node8 -> Node14
	Node9 [label="{{9|2}}"];	Node9 -> Node15	Node9 -> Node16
	Node10 [label="{{10|t|1}|0011}}"];
	Node11 [label="{{11|c|1}|0100}}"];
	Node12 [label="{{12|s|1}|0101}}"];
	Node13 [label="{{13|h|1}|0110}}"];
	Node14 [label="{{14|r|1}|0111}}"];
	Node15 [label="{{15|n|1}|00100}}"];
	Node16 [label="{{16|_|1}|00101}}"];
	Node0 [color=lawngreen];
	label="Changed node weight for 0 by 1."
}}
```