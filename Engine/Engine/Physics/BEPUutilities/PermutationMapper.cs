﻿namespace Engine.Physics.BEPUutilities
{
    /// <summary>
    /// Maps indices to permuted versions of the indices.
    /// </summary>
    public class PermutationMapper
    {
        private static long[] primes =
        {
            818660357, 878916037, 844828463, 706609493, 478906601, 707908823, 938052293, 630235027, 984165979,
            522311087,
            533822657, 647031821, 756030427, 649614073, 988123237, 367570499, 906500941, 996040853, 783408599,
            547916219,
            874973537, 895987171, 993400939, 370087031, 796455917, 681976277, 952539209, 787319591, 693639263,
            400306397,
            464911147, 632819419, 535101863, 989444723, 758640511, 838280689, 368828611, 587720411, 564588593,
            967023193,
            667720633, 806896583, 423056789, 517196051, 932791141, 859241111, 560735977, 636693319, 439523569,
            486546097,
            607018589, 809508331, 743003231, 658666247, 468722633, 915685681, 880227137, 819967303, 901242241,
            778194047,
            741702719, 482725577, 926205001, 927520481, 402832763, 886794539, 452206037, 753430547, 876291181,
            485272951,
            998674531, 648322789, 761247251, 804290869, 864486559, 410411999, 812125621, 801673849, 425581939,
            934104421,
            462373111, 643151417, 685862539, 550475501, 770369947, 635404883, 884166113, 863178133, 852689941,
            833050909,
            956490817, 527424127, 671607571, 759943181, 553033951, 519748331, 871040311, 481451183, 457284019,
            387695681,
            733904803, 782106799, 736506781, 949902923, 484000151, 948584543, 386437679, 928838927, 771671671,
            405356321,
            436988143, 412937689, 723496063, 965703469, 650905219, 438255473, 772975039, 698824649, 491646619,
            728694209,
            351253391, 573576683, 897302563, 383915503, 524866219, 793842233, 970974217, 986803099, 589008533,
            670310803,
            521029501, 661249651, 678087929, 381394511, 523586243, 654784799, 892050463, 727394567, 414200539,
            823889863,
            595433441, 951221021, 429383393, 789927269, 726096121, 881542477, 776892307, 697527421, 453478561,
            461099827,
            722198021, 504411067, 994722161, 700123451, 385176713, 510802769, 890732693, 696225749, 749524037,
            662541689,
            974934083, 394001009, 657371381, 443321381, 865796509, 748217497, 702715747, 684569971, 774280471,
            992083457,
            537661367, 382653827, 622495889, 631525019, 480174977, 391478107, 495469771, 442056463, 616040899,
            618625193,
            358782341, 586434157, 869729963, 473817257, 939368383, 363804941, 665132051, 969657991, 985485797,
            357526417,
            763851047, 528702827, 968340739, 715704307, 840899977, 554317237, 603155899, 976257697, 914373869,
            972295381,
            506966203, 428116781, 919629439, 458558563, 909124627, 683276981, 843515081, 659956823, 567155357,
            982852639,
            366315427, 786014507, 421791619, 471267781, 644447959, 711804833, 513360119, 545353273, 501854377,
            846137323,
            430651127, 732601613, 694931981, 765159553, 565870867, 361296113, 407881267, 910434589, 805593809,
            918318551,
            754734557, 509521151, 515920621, 627653659, 469997687, 913055509, 641857133, 746917957, 555602107,
            580002967,
            500573561, 990762767, 731298989, 350000041, 977572727, 959119229, 445860881, 835666891, 404093659,
            467452939,
            388955857, 417993047, 817348793, 415462661, 882852679, 752128019, 705313919, 508244797, 877605947,
            894672263,
            614753233, 924890411, 851377763, 621205957, 569722943, 836976167, 605729783, 750824423, 997360631,
            719601319,
            399047321, 735204647, 978895943, 899934323, 674196121, 434451827, 352509701, 687156553, 779500681,
            581290511,
            898615969, 556885871, 839592911, 656079659, 861868031, 672901241, 653492839, 449666827, 740406013,
            680680703,
            544070951, 499298507, 850065869, 612177563, 936733703, 559452917, 463639243, 435719233, 769068361,
            476365079,
            675495157, 645738493, 831744161, 625074937, 406619023, 601868711, 419260421, 902559319, 701418559,
            800370419,
            757334933, 356270183, 704014501, 610886447, 980214871, 424321201, 420528029, 532542931, 745611751,
            856617583,
            512081189, 526147967, 360037807, 609597161, 541503811, 888105241, 868419913, 676791293, 953855899,
            477635657,
            945954917, 889419229, 922256729, 617333111, 872353393, 571010317, 689747717, 540223237, 594146089,
            583856813,
            596715221, 784713949, 720899299, 943323247, 931473479, 531262091, 791230073, 558167293, 619913999,
            847450421,
            472541753, 930158309, 652199311, 893358073, 955169563, 498021431, 634111459, 542787121, 448395943,
            494196701,
            737807227, 714400243, 923575657, 568439609, 822581623, 829125467, 397785029, 392735821, 724793233,
            762545383,
            691041023, 853997549, 903872867, 487820351, 574863577, 710504833, 780803633, 378881311, 640566593,
            981532709,
            431916809, 608306243, 375114847, 433186519, 964385563, 371347091, 549197707, 365063081, 663835979,
            713100181,
            885483343, 489099293, 409146953, 447130181, 827817959, 857928371, 816045233, 496745537, 454747417,
            411672083,
            459827497, 717003281, 813433657, 860554531, 503134721, 591574051, 373863263, 538942241, 637982731,
            578719403,
            766467371, 599295517, 848757323, 466181593, 396524683, 426850301, 688452473, 826508399, 585146633,
            814739587,
            623787599, 380139107, 744307493, 372605839, 855307913, 577432501, 808205117, 810815693, 626368511,
            709205743,
            916999177, 598005553, 944636591, 590290153, 905189147, 730000753, 830433151, 492922333, 957803549,
            718302763,
            973615067, 376372811, 669017941, 444590423, 960432989, 592860803, 582574117, 940688839, 377628311,
            788628949,
            536379929, 963067373, 907810073, 600582071, 792537247, 395258257, 961750859, 795149801, 362549689,
            873662401,
            797757101, 546635237, 834358871, 935417807, 613460527, 456014303, 604437347, 551755871, 562018817,
            639275389,
            529981651, 505691939, 842209411, 821274059, 799060477, 563302513, 666426347, 572296079, 355016209,
            514641793,
            942005767, 825202369, 739107877, 867110077, 440788573, 911746061, 628943891, 518470987, 775583663,
            767767657,
            576150287, 353763589, 416727907, 390218303, 920939597, 692337133, 450937237, 679382051, 802984639,
            947268481,
            490370093, 401570791, 475092199
        };


        private long currentOffset;
        private long currentPrime;

        private long permutationIndex;

        /// <summary>
        /// Constructs a new permutation mapper.
        /// </summary>
        public PermutationMapper()
        {
            PermutationIndex = 0;
        }

        /// <summary>
        /// Gets or sets the permutation index used by the solver.  If the simulation is restarting from a given frame,
        /// setting this index to be consistent is required for deterministic results.
        /// </summary>
        public long PermutationIndex
        {
            get => permutationIndex;
            set
            {
                permutationIndex = value < 0 ? value + long.MaxValue + 1 : value;
                currentPrime = primes[permutationIndex % primes.Length];

                currentOffset = currentPrime * permutationIndex;

                if (currentOffset < 0)
                {
                    currentOffset = currentOffset + long.MaxValue + 1;
                }
            }
        }


        /// <summary>
        /// Gets a remapped index.
        /// </summary>
        /// <param name="index">Original index of an element in the set to be redirected to a shuffled position.</param>
        /// <param name="setSize">Size of the set being permuted. Must be smaller than 350000041.</param>
        /// <returns>The remapped index.</returns>
        public long GetMappedIndex(long index, int setSize)
        {
            return (index * currentPrime + currentOffset) % setSize;
        }

        /// <summary>
        /// Gets a remapped index.
        /// </summary>
        /// <param name="index">Original index of an element in the set to be redirected to a shuffled position.</param>
        /// <param name="setSize">Size of the set being permuted. Must be smaller than 350000041.</param>
        /// <returns>The remapped index.</returns>
        public int GetMappedIndex(int index, int setSize)
        {
            return (int) ((index * currentPrime + currentOffset) % setSize);
        }
    }
}