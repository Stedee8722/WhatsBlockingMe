using GDWeave.Godot;
using GDWeave.Godot.Variants;
using GDWeave.Modding;

namespace WhatsBlockingMe
{
    internal class LoadingMenuScript : IScriptMod
    {
        public IEnumerable<Token> Modify(string path, IEnumerable<Token> tokens)
        {
            // extend
            var extendsWaiter = new MultiTokenWaiter([
                t => t.Type is TokenType.PrExtends,
                t => t.Type is TokenType.Newline
            ], allowPartialMatch: true);
            
            // t >= 4:
            var beatCounterWaiter = new MultiTokenWaiter([
                t => t.Type is TokenType.CfIf,
                t => t is IdentifierToken {Name:"t"},
                t => t.Type is TokenType.OpGreaterEqual,
                t => t is ConstantToken {Value: IntVariant {Value: 4}},
                t => t.Type is TokenType.Colon
            ]);
            
            // var packets_recieved = 0
            var initWaiter = new MultiTokenWaiter([
                t => t.Type is TokenType.PrVar,
                t => t is IdentifierToken {Name:"packets_recieved"},
                t => t.Type is TokenType.OpAssign,
                t => t is ConstantToken {Value: IntVariant {Value: 0}},
                t => t.Type is TokenType.Newline && t.AssociatedData == 2
            ]);

            // packets_recieved += 1
            var forLoopWaiter = new MultiTokenWaiter([
                t => t is IdentifierToken {Name:"packets_recieved"},
                t => t.Type is TokenType.OpAssignAdd,
                t => t is ConstantToken {Value: IntVariant {Value: 1}},
                t => t.Type is TokenType.Newline && t.AssociatedData == 2
            ]);

            var removeFromListWaiter = new MultiTokenWaiter([
                t => t.Type is TokenType.CfIf,
                t => t is IdentifierToken {Name:"Network"},
                t => t.Type is TokenType.Period,
                t => t is IdentifierToken {Name:"FLUSH_PACKET_INFORMATION"},
                t => t.Type is TokenType.BracketOpen,
                t => t is IdentifierToken {Name:"key"},
                t => t.Type is TokenType.BracketClose,
                t => t.Type is TokenType.OpGreater,
                t => t is ConstantToken {Value: IntVariant {Value: 0}},
                t => t.Type is TokenType.Colon,
                t => t.Type is TokenType.Newline && t.AssociatedData == 4
            ]);

            foreach (var token in tokens)
            {
                if (extendsWaiter.Check(token))
                {
                    Console.WriteLine("init var b");
                    yield return token;

                    // var b = 0
                    yield return new Token(TokenType.PrVar);
                    yield return new IdentifierToken("b");
                    yield return new Token(TokenType.OpAssign);
                    yield return new ConstantToken(new IntVariant(0));
                    yield return new Token(TokenType.Newline);
                }
                else if (beatCounterWaiter.Check(token))
                {
                    Console.WriteLine("Counting var b");
                    yield return token;

                    // b += 1
                    yield return new Token(TokenType.Newline, 2);
                    yield return new IdentifierToken("b");
                    yield return new Token(TokenType.OpAssignAdd);
                    yield return new ConstantToken(new IntVariant(1));
                    yield return new Token(TokenType.Newline, 2);

                }
                else if (initWaiter.Check(token))
                {
                    Console.WriteLine("init checks");
                    yield return token;

                    // var player = ""
                    yield return new Token(TokenType.PrVar);
                    yield return new IdentifierToken("player");
                    yield return new Token(TokenType.OpAssign);
                    yield return new ConstantToken(new StringVariant(""));
                    yield return new Token(TokenType.Newline, 2);

                    // var packets = []
                    yield return new Token(TokenType.PrVar);
                    yield return new IdentifierToken("packets");
                    yield return new Token(TokenType.OpAssign);
                    yield return new Token(TokenType.BracketOpen);
                    yield return new Token(TokenType.BracketClose);
                    yield return new Token(TokenType.Newline, 2);

                    // var not_sent = []
                    yield return new Token(TokenType.PrVar);
                    yield return new IdentifierToken("not_sent");
                    yield return new Token(TokenType.OpAssign);
                    yield return new Token(TokenType.BracketOpen);
                    yield return new Token(TokenType.BracketClose);
                    yield return new Token(TokenType.Newline, 2);

                    // for member in Network.LOBBY_MEMBERS:
                    yield return new Token(TokenType.CfFor);
                    yield return new IdentifierToken("member");
                    yield return new Token(TokenType.OpIn);
                    yield return new IdentifierToken("Network");
                    yield return new Token(TokenType.Period);
                    yield return new IdentifierToken("LOBBY_MEMBERS");
                    yield return new Token(TokenType.Colon);
                    yield return new Token(TokenType.Newline, 3);

                    // not_sent.append(member["steam_id"])
                    yield return new IdentifierToken("not_sent");
                    yield return new Token(TokenType.Period);
                    yield return new IdentifierToken("append");
                    yield return new Token(TokenType.ParenthesisOpen);
                    yield return new IdentifierToken("member");
                    yield return new Token(TokenType.BracketOpen);
                    yield return new ConstantToken(new StringVariant("steam_id"));
                    yield return new Token(TokenType.BracketClose);
                    yield return new Token(TokenType.ParenthesisClose);
                    yield return new Token(TokenType.Newline, 2);
                }
                else if (forLoopWaiter.Check(token))
                {
                    Console.WriteLine("injects code");
                    yield return token;

                    // if Network.STEAM_ID == key:
                    yield return new Token(TokenType.Newline, 3);
                    yield return new Token(TokenType.CfIf);
                    yield return new IdentifierToken("Network");
                    yield return new Token(TokenType.Period);
                    yield return new IdentifierToken("STEAM_ID");
                    yield return new Token(TokenType.OpEqual);
                    yield return new IdentifierToken("key");
                    yield return new Token(TokenType.Colon);
                    yield return new Token(TokenType.Newline, 4);

                    // not_sent.erase(key)
                    yield return new IdentifierToken("not_sent");
                    yield return new Token(TokenType.Period);
                    yield return new IdentifierToken("erase");
                    yield return new Token(TokenType.ParenthesisOpen);
                    yield return new IdentifierToken("key");
                    yield return new Token(TokenType.ParenthesisClose);
                    yield return new Token(TokenType.Newline, 2);

                    // $CenterContainer / Label.text = "         " + $CenterContainer / Label.text + "\nReceived: " + str(packets_recieved) + " | Size: " + str(Network.LOBBY_MEMBERS.size())
                    yield return new Token(TokenType.Dollar);
                    yield return new IdentifierToken("CenterContainer");
                    yield return new Token(TokenType.OpDiv);
                    yield return new IdentifierToken("Label");
                    yield return new Token(TokenType.Period);
                    yield return new IdentifierToken("text");
                    yield return new Token(TokenType.OpAssign);
                    yield return new ConstantToken(new StringVariant("         "));
                    yield return new Token(TokenType.OpAdd);
                    yield return new Token(TokenType.Dollar);
                    yield return new IdentifierToken("CenterContainer");
                    yield return new Token(TokenType.OpDiv);
                    yield return new IdentifierToken("Label");
                    yield return new Token(TokenType.Period);
                    yield return new IdentifierToken("text");
                    yield return new Token(TokenType.OpAdd);
                    yield return new ConstantToken(new StringVariant("\nReceived: "));
                    yield return new Token(TokenType.OpAdd);
                    yield return new Token(TokenType.BuiltInFunc, (uint?)BuiltinFunction.TextStr);
                    yield return new Token(TokenType.ParenthesisOpen);
                    yield return new IdentifierToken("packets_recieved");
                    yield return new Token(TokenType.ParenthesisClose);
                    yield return new Token(TokenType.OpAdd);
                    yield return new ConstantToken(new StringVariant(" | Size: "));
                    yield return new Token(TokenType.OpAdd);
                    yield return new Token(TokenType.BuiltInFunc, (uint?)BuiltinFunction.TextStr);
                    yield return new Token(TokenType.ParenthesisOpen);
                    yield return new IdentifierToken("Network");
                    yield return new Token(TokenType.Period);
                    yield return new IdentifierToken("LOBBY_MEMBERS");
                    yield return new Token(TokenType.Period);
                    yield return new IdentifierToken("size");
                    yield return new Token(TokenType.ParenthesisOpen);
                    yield return new Token(TokenType.ParenthesisClose);
                    yield return new Token(TokenType.ParenthesisClose);
                    yield return new Token(TokenType.Newline, 2);

                    // for id in not_sent:
                    yield return new Token(TokenType.CfFor);
                    yield return new IdentifierToken("id");
                    yield return new Token(TokenType.OpIn);
                    yield return new IdentifierToken("not_sent");
                    yield return new Token(TokenType.Colon);
                    yield return new Token(TokenType.Newline, 3);

                    // player = player + "\n" + Network._get_username_from_id(int(id))
                    yield return new IdentifierToken("player");
                    yield return new Token(TokenType.OpAssign);
                    yield return new IdentifierToken("player");
                    yield return new Token(TokenType.OpAdd);
                    yield return new ConstantToken(new StringVariant("\n"));
                    yield return new Token(TokenType.OpAdd);
                    yield return new IdentifierToken("Network");
                    yield return new Token(TokenType.Period);
                    yield return new IdentifierToken("_get_username_from_id");
                    yield return new Token(TokenType.ParenthesisOpen);
                    yield return new Token(TokenType.BuiltInType, 2);
                    yield return new Token(TokenType.ParenthesisOpen);
                    yield return new IdentifierToken("id");
                    yield return new Token(TokenType.ParenthesisClose);
                    yield return new Token(TokenType.ParenthesisClose);
                    yield return new Token(TokenType.Newline, 2);

                    // $CenterContainer / Label.text = $CenterContainer / Label.text + "\n\n" + "Waiting for:" + player
                    yield return new Token(TokenType.Dollar);
                    yield return new IdentifierToken("CenterContainer");
                    yield return new Token(TokenType.OpDiv);
                    yield return new IdentifierToken("Label");
                    yield return new Token(TokenType.Period);
                    yield return new IdentifierToken("text");
                    yield return new Token(TokenType.OpAssign);
                    yield return new Token(TokenType.Dollar);
                    yield return new IdentifierToken("CenterContainer");
                    yield return new Token(TokenType.OpDiv);
                    yield return new IdentifierToken("Label");
                    yield return new Token(TokenType.Period);
                    yield return new IdentifierToken("text");
                    yield return new Token(TokenType.OpAdd);
                    yield return new ConstantToken(new StringVariant("\n\nWaiting for:"));
                    yield return new Token(TokenType.OpAdd);
                    yield return new IdentifierToken("player");
                    yield return new Token(TokenType.Newline, 2);
                }
                else if (removeFromListWaiter.Check(token))
                {
                    Console.WriteLine("injects remove code");
                    yield return token;

                    // not_sent.erase(key)
                    yield return new IdentifierToken("not_sent");
                    yield return new Token(TokenType.Period);
                    yield return new IdentifierToken("erase");
                    yield return new Token(TokenType.ParenthesisOpen);
                    yield return new IdentifierToken("key");
                    yield return new Token(TokenType.ParenthesisClose);
                    yield return new Token(TokenType.Newline, 4);
                }
                else
                {
                    yield return token;
                }
            }
        }

        public bool ShouldRun(string path) => path == "res://Scenes/Menus/Loading Menu/loading_menu.gdc";
    }
}
