using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sociatis.Models.Congress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sociatis_Unit_Tests
{
    [TestClass]
    public class CongressStartVotingViewModelTests
    {

        public CongressStartVotingViewModelTests()
        {

        }

        [TestMethod]
        public void EmbedPostVoteTest()
        {
            CongressStartVotingViewModel viewVM = new CongressStartVotingViewModel();
            CongressVotingViewModel votingVM = new ChangeCongressCadenceLengthViewModel();

            viewVM.EmbedPostVote(votingVM);

            Assert.AreEqual(votingVM, viewVM.EmbeddedVote);
            Assert.AreEqual((int)votingVM.VotingType, viewVM.SelectedVotingTypeID);
        }
    }
}
