﻿/****************************************************************************************
*	This code originates from the software development department at					*
*	swedish insurance and private loan broker Insplanet AB.								*
*	Full license available in license.txt												*
*	This text block may not be removed or altered.                                  	*
*	The list of contributors may be extended.                                           *
*																						*
*							Mikael Axblom, head of software development, Insplanet AB	*
*																						*
*	Contributors: Mikael Axblom															*
*****************************************************************************************/
using System;
using System.Web.Mvc;
using SystemHealthExternalInterface;
using HealthAndAuditShared;
using Newtonsoft.Json;


namespace ControlCentre.Controllers
{
    public class UpsertRuleController : Controller
    {
        // GET: UpsertRule
        [HttpGet]
        public ActionResult Index()
        {
            return View("ruleview");
        }

        [HttpGet]
        public ActionResult ViewRule(string id)
        {
            ViewBag.documentid = id;

            AnalyseRuleset rule;
            return GetRule(id, out rule) ? View("ruleview", rule) : View("ruleview");

        }

        [HttpPost]
        [ActionName("upsert")]
        public ActionResult Upsert(string serializedRule)
        {
            try
            {
                var typeCheckConvert = JsonConvert.DeserializeObject<Fuling>(serializedRule);
                var realType = typeCheckConvert.RealType;
                var deseralizedRule = JsonConvert.DeserializeObject(serializedRule, realType);
                if(deseralizedRule is AnalyseRuleset)
                {
                    HelperMethods.GetRuleStorage().UpsertRuleSet(deseralizedRule as AnalyseRuleset);
                }
            }
            catch (Exception ex)
            {
                ViewBag.pageException = HelperMethods.FormatException(ex);
            }
            return View("ruleview");
        }

        //I need some way to get the real type from the ruleset, but since it's abstract this "pretty "little work-around is done.
        private class Fuling : AnalyseRuleset
        {
            public override bool AddAndCheckIfTriggered(SystemEvent opResult)
            {
                throw new NotImplementedException();
            }
        }


        private bool GetRule(string ruleID, out AnalyseRuleset rule)
        {
            rule = null;
            try
            {
                rule = HelperMethods.GetRuleStorage().GetRuleByID(ruleID);
                return true;
            }
            catch (Exception ex)
            {
                ViewBag.pageException = HelperMethods.FormatException(ex);
                return false;
            }
        }

    }
}