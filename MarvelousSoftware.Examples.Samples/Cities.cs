using System;
using System.Collections.Generic;
using System.Text;

namespace MarvelousSoftware.Examples.Samples
{
    public class Cities
    {
        private static Random _random = new Random();

        public static List<City> Get(DateTime? created = null)
        {
            var createdDate = created ?? DateTime.Now;

            var list = new List<City>
            {
                new City("Shanghai", 24150000, 6340.5, 3809, "China", GetDate(createdDate), true),
                new City("Karachi", 23500000, 3527, 6663, "Pakistan", GetDate(createdDate), true),
                new City("Beijing", 21516000, 16410.54, 1311, "China", GetDate(createdDate), true),
                new City("Tianjin", 14722100, 4037, 3647, "China", GetDate(createdDate), true),
                new City("Istanbul", 14377019, 5461, 2633, "Turkey", GetDate(createdDate), false),
                new City("Lagos", 13400000, 999.58, 13406, "Nigeria", GetDate(createdDate), true),
                new City("Tokyo", 13297629, 2189, 6075, "Japan", GetDate(createdDate), true),
                new City("Guangzhou", 12700800, 3843.43, 3305, "China", GetDate(createdDate), true),
                new City("Mumbai", 12655220, 603.4, 20973, "India", GetDate(createdDate), true),
                new City("Moscow", 12197596, 2510.12, null, "Russia", GetDate(createdDate), true),
                new City("Dhaka", 12043977, 1463.6, 8229, "Bangladesh", GetDate(createdDate)),
                new City("Cairo", 11922949, 3085.1, 3865, "Egypt", GetDate(createdDate), true),
                new City("São Paulo", 11895893, 1521.11, 7821, "Brazil", GetDate(createdDate), false),
                new City("Lahore", 11318745, 1772, 6388, "Pakistan", GetDate(createdDate), true),
                new City("Shenzhen", 10467400, 1991.64, 5256, "China", GetDate(createdDate), true),
                new City("Seoul", 10388055, 605.21, 17164, "South Korea", GetDate(createdDate), true),
                new City("Jakarta", 9988329, 664.12, 1504, "Indonesia", GetDate(createdDate), true),
                new City("Kinshasa", 9735000, 1117.62, 871, "Democratic Republic of the Congo", GetDate(createdDate)),
                new City("Mexico City", 8874724, 1485.49, 5974, "Mexico", GetDate(createdDate), false),
                new City("Lima", 8693387, 2672.3, 3253, "Peru", GetDate(createdDate)),
                new City("New York City", 8491079, 783.84, 10833, "United States", GetDate(createdDate), true),
                new City("Bengaluru", 8425970, 709.5, 11876, "India", GetDate(createdDate), true),
                new City("London", 8416500, 1572.15, 5353, "United Kingdom", GetDate(createdDate), false),
                new City("Bangkok", 8280925, 1568.74, 5279, "Thailand", GetDate(createdDate), true),
                new City("Dongguan", 8220207, 2469.4, 3329, "China", GetDate(createdDate), true),
                new City("Chongqing", 8189800, 5473, null, "China", GetDate(createdDate), true),
                new City("Nanjing", 8187828, 4713.85, 1737, "China", GetDate(createdDate), true),
                new City("Tehran", 8154051, 686, 11886, "Iran", GetDate(createdDate), true),
                new City("Shenyang", 8106171, 12942, null, "China", GetDate(createdDate), true),
                new City("Ahmedabad", 8029975, 475, 16905, "India", GetDate(createdDate), true),
                new City("Bogotá", 7776845, 859.11, 9052, "Colombia", GetDate(createdDate)),
                new City("Ho Chi Minh City", 7681700, 2095.6, 3666, "Vietnam", GetDate(createdDate), true),
                new City("Ningbo", 7605689, 9816.23, 775, "China", GetDate(createdDate), true),
                new City("Hong Kong", 7219700, 1104.43, 6537, "China", GetDate(createdDate), true),
                new City("Baghdad", 7180889, 4555, 1576, "Iraq", GetDate(createdDate), true),
                new City("Changsha", 7044118, 11819, 596, "China", GetDate(createdDate), true),
                new City("Wuhan", 6886253, 1327.61, 5187, "China", GetDate(createdDate), true),
                new City("Hyderabad", 6809970, 621.48, 10958, "India", GetDate(createdDate), true),
                new City("Hanoi", 6844100, 3323.6, 2059, "Vietnam", GetDate(createdDate), true),
                new City("Rio de Janeiro", 6429923, 1200.27, 5357, "Brazil", GetDate(createdDate), false),
                new City("Foshan", 6151622, 2034.62, 3023, "China", GetDate(createdDate), true),
                new City("Santiago", 5743719, 1249.9, 4595, "Chile", GetDate(createdDate), false),
                new City("Riyadh", 5676621, 1233.98, 46, "Saudi Arabia", GetDate(createdDate)),
                new City("Singapore", 5399200, 712.4, 7579, "Singapore", GetDate(createdDate), true),
                new City("Shantou", 5391028, 2064.42, 2611, "China", GetDate(createdDate), true),
                new City("Yangon", 5214000, 598.75, 8708, "Burma", GetDate(createdDate)),
                new City("Saint Petersburg", 5191690, 1439, 3608, "Russia", GetDate(createdDate), true),
                new City("Ankara", 5150072, 1910.92, 2695, "Turkey", GetDate(createdDate), false),
                new City("Pune", 5049968, 450.69, 11205, "India", GetDate(createdDate), true),
                new City("Chennai", 4792949, 426.51, 11238, "India", GetDate(createdDate), true),
                new City("Abidjan", 4765000, 2119, 2249, "Ivory Coast", GetDate(createdDate)),
                new City("Chengdu", 4741929, 421, 11263, "China", GetDate(createdDate), true),
                new City("Alexandria", 4616625, 2300, 2007, "Egypt", GetDate(createdDate), true),
                new City("Kolkata", 4486679, 200.7, 22355, "India", GetDate(createdDate), true),
                new City("Xi'an   ", 4467837, 832.17, 5369, "China", GetDate(createdDate), true),
                new City("Surat", 4462002, 326.515, 13666, "India", GetDate(createdDate), true),
                new City("Johannesburg", 4434827, 1644.98, 2696, "South Africa", GetDate(createdDate)),
                new City("Dar es Salaam", 4364541, 1631.12, 2676, "Tanzania", GetDate(createdDate)),
                new City("Suzhou", 4327066, 1649.72, 2623, "China", GetDate(createdDate), true),
                new City("Harbin", 4280701, 1718.2, 2491, "China", GetDate(createdDate), true),
                new City("Giza", 4239988, 289.08, 14667, "Egypt", GetDate(createdDate), true),
                new City("İzmir", 4113072, 1910.92, 2152, "Turkey", GetDate(createdDate), false),
                new City("Zhengzhou", 4122087, 1015.66, 4059, "China", GetDate(createdDate), true),
                new City("New Taipei City", 3954929, 2052.57, 1927, "Taiwan", GetDate(createdDate), true),
                new City("Los Angeles", 3884307, 1213.85, 32, "United States", GetDate(createdDate), true),
                new City("Cape Town", 3740026, 2444.97, 153, "South Africa", GetDate(createdDate)),
                new City("Yokohama", 3680267, 437.38, 8414, "Japan", GetDate(createdDate), true),
                new City("Busan", 3590101, 766.12, 4686, "South Korea", GetDate(createdDate), true),
                new City("Hangzhou", 3560391, 728.19, 4889, "China", GetDate(createdDate), true),
                new City("Xiamen", 3531347, 1699, 2078, "China", GetDate(createdDate), true)
            };
            return list;
        }

        private static DateTime GetDate(DateTime baseDate, int minutes = 90000)
        {
            var m = _random.Next(minutes);
            return baseDate.AddMinutes(m * -1);
        }

        public string TableToCode()
        {
            var input = @"new City('	Shanghai	',	24150000[6]	,	6340.5	,	3809	, '	 China	'),
new City('	Karachi	',	23500000[9]	,	3527	,	6663	, '	 Pakistan	'),
new City('	Beijing	',	21516000[11]	,	16410.54	,	1311	, '	 China	'),
new City('	Tianjin	',	14722100[13]	,	4037[15]	,	3647	, '	 China	'),
new City('	Istanbul	',	14377019[16]	,	5461[18]	,	2633	, '	 Turkey	'),
new City('	Lagos	',	13400000[19]	,	999.58[21]	,	13406	, '	 Nigeria	'),
new City('	Tokyo	',	13297629[22]	,	2189[23]	,	6075	, '	 Japan	'),
new City('	Guangzhou	',	12700800[24]	,	3843.43	,	3305	, '	 China	'),
new City('	Mumbai	',	12655220[26]	,	603.4	,	20973	, '	 India	'),
new City('	Moscow	',	12197596[28]	,	2510.12[31]	,	4859	, '	 Russia	'),
new City('	Dhaka	',	12043977[32]	,	1463.6[33]	,	8229	, '	 Bangladesh	'),
new City('	Cairo	',	11922949[34]	,	3085.1[36]	,	3865	, '	 Egypt	'),
new City('	São Paulo	',	11895893[37]	,	1521.11	,	7821	, '	 Brazil	'),
new City('	Lahore	',	11318745[39]	,	1772[39]	,	6388	, '	 Pakistan	'),
new City('	Shenzhen	',	10467400[40]	,	1991.64[41]	,	5256	, '	 China	'),
new City('	Seoul	',	10388055[42]	,	605.21[43]	,	17164	, '	 South Korea	'),
new City('	Jakarta	',	9988329[44]	,	664.12	,	1504	, '	 Indonesia	'),
new City('	Kinshasa	',	9735000[46][47][full citation needed]	,	1117.62[49]	,	871	, '	 Democratic Republic of the Congo	'),
new City('	Mexico City	',	8874724[50]	,	1485.49[51]	,	5974	, '	 Mexico	'),
new City('	Lima	',	8693387[52]	,	2672.3	,	3253	, '	 Peru	'),
new City('	New York City	',	8491079[54]	,	783.84	,	10833	, '	 United States	'),
new City('	Bengaluru	',	8425970[26]	,	709.5[56]	,	11876	, '	 India	'),
new City('	London	',	8416500[57]	,	1572.15	,	5353	, '	 United Kingdom	'),
new City('	Bangkok	',	8280925[59]	,	1568.74	,	5279	, '	 Thailand	'),
new City('	Dongguan	',	8220207[61]	,	2469.4	,	3329	, '	 China	'),
new City('	Chongqing	',	8189800[a]	,	5473[c]	,	1496	, '	 China	'),
new City('	0	',	0	,	0	,	0	, '	0	'),
new City('	0	',	0	,	0	,	0	, '	0	'),
new City('	Nanjing	',	8187828[66]	,	4713.85	,	1737	, '	 China	'),
new City('	Tehran	',	8154051[67]	,	686[69]	,	11886	, '	 Iran	'),
new City('	Shenyang	',	8106171[70]	,	12942	,	626	, '	 China	'),
new City('	Ahmedabad	',	8029975[26]	,	475	,	16905	, '	 India	'),
new City('	Bogotá	',	7776845[71]	,	859.11[73][74]	,	9052	, '	 Colombia	'),
new City('	Ho Chi Minh City	',	7681700[75]	,	2095.6[76]	,	3666	, '	 Vietnam	'),
new City('	Ningbo	',	7605689[77]	,	9816.23	,	775	, '	 China	'),
new City('	Hong Kong	',	7219700[78]	,	1104.43[80]	,	6537	, '	 China	'),
new City('	Baghdad	',	7180889[81]	,	4555[83]	,	1576	, '	 Iraq	'),
new City('	Changsha	',	7044118	,	11819	,	596	, '	 China	'),
new City('	Wuhan	',	6886253[85]	,	1327.61[87]	,	5187	, '	 China	'),
new City('	Hyderabad	',	6809970[26]	,	621.48[88]	,	10958	, '	 India	'),
new City('	Hanoi	',	6844100[75]	,	3323.6[76]	,	2059	, '	 Vietnam	'),
new City('	Rio de Janeiro	',	6429923[37]	,	1200.27[89]	,	5357	, '	 Brazil	'),
new City('	Foshan	',	6151622[90][91]	,	2034.62[92]	,	3023	, '	 China	'),
new City('	Santiago	',	5743719[93]	,	1249.9	,	4595	, '	 Chile	'),
new City('	Riyadh	',	5676621[95]	,	1233.98[97]	,	46	, '	 Saudi Arabia	'),
new City('	Singapore	',	5399200[98]	,	712.4	,	7579	, '	 Singapore	'),
new City('	Shantou	',	5391028[99]	,	2064.42	,	2611	, '	 China	'),
new City('	Yangon	',	5214000[100]	,	598.75[102]	,	8708	, '	 Burma	'),
new City('	Saint Petersburg	',	5191690[103]	,	1439[31]	,	3608	, '	 Russia	'),
new City('	Ankara	',	5150072[104]	,	1910.92	,	2695	, '	 Turkey	'),
new City('	Pune	',	5049968[26]	,	450.69	,	11205	, '	 India	'),
new City('	Chennai	',	4792949[106]	,	426.51[108]	,	11238	, '	 India	'),
new City('	Abidjan	',	4765000[46]	,	2119[109]	,	2249	, '	 Ivory Coast	'),
new City('	Chengdu	',	4741929[110]	,	421[110]	,	11263	, '	 China	'),
new City('	Alexandria	',	4616625[34]	,	2300[111][112]	,	2007	, '	 Egypt	'),
new City('	Kolkata	',	4486679[26]	,	200.7	,	22355	, '	 India	'),
new City('	Xi'an	',	4467837[113]	,	832.17	,	5369	, '	 China	'),
new City('	Surat	',	4462002[26]	,	326.515[115]	,	13666	, '	 India	'),
new City('	Johannesburg	',	4434827[116]	,	1644.98	,	2696	, '	 South Africa	'),
new City('	Dar es Salaam	',	4364541[117]	,	1631.12[119]	,	2676	, '	 Tanzania	'),
new City('	Suzhou	',	4327066[120]	,	1649.72[122]	,	2623	, '	 China	'),
new City('	Harbin	',	4280701[123][124]	,	1718.2[125]	,	2491	, '	 China	'),
new City('	Giza	',	4239988[126]	,	289.08	,	14667	, '	 Egypt	'),
new City('	İzmir	',	4113072[104]	,	1910.92	,	2152	, '	 Turkey	'),
new City('	Zhengzhou	',	4122087[129]	,	1015.66	,	4059	, '	 China	'),
new City('	New Taipei City	',	3954929[131]	,	2052.57[132]	,	1927	, '	 Taiwan	'),
new City('	Los Angeles	',	3884307[54]	,	1213.85	,	32	, '	 United States	'),
new City('	Cape Town	',	3740026[116]	,	2444.97	,	153	, '	 South Africa	'),
new City('	Yokohama	',	3680267[134]	,	437.38	,	8414	, '	 Japan	'),
new City('	Busan	',	3590101[135]	,	766.12[135]	,	4686	, '	 South Korea	'),
new City('	Hangzhou	',	3560391[136]	,	728.19[136]	,	4889	, '	 China	'),
new City('	Xiamen	',	3531347[137]	,	1699	,	2078	, '	 China	'),


";

            var result = new StringBuilder(input.Length);

            var withinBracket = false;
            foreach (var current in input)
            {
                if (current == '[')
                {
                    withinBracket = true;
                    continue;
                }
                if (current == ']')
                {
                    withinBracket = false;
                    continue;
                }
                if (withinBracket)
                {
                    continue;
                }

                result.Append(current);
            }

            return result.ToString();
        }
    }
}