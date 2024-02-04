using System.Collections.Generic;

namespace Croffle.HttpWaffle
{
    /// <summary>
    ///waffle page를 List로 정리
    /// </summary>
    internal class WafflePage
    {
        static readonly private List<string> pages = new List<string>
            {
                "https://waffle.wku.ac.kr/lms/myclass/index.jsp",
                "https://waffle.wku.ac.kr/lms/myclass/classes.jsp",
                "https://waffle.wku.ac.kr/lms/myclass/attend/attend.jsp",
                "https://waffle.wku.ac.kr/lms/myclass/addition/replay.jsp",
                "https://waffle.wku.ac.kr/lms/myclass/addition/attach.jsp",
                "https://waffle.wku.ac.kr/lms/myclass/lecture/lecture_movie",
                "https://waffle.wku.ac.kr/lms/myclass/lecture/lecture_hwork_act",
                "https://waffle.wku.ac.kr/lms/return.jsp"
            };

        static readonly private List<string> page_post = new List<string>
            {
                "unaf/lms/lctr:lctr_S11",
                "unaf/lms/lctrbrdpost:lctrBrdPost_S03"
            };

        /// <summary>
        ///waffle page를 List로 정리
        /// </summary>
        public WafflePage() { }

        /// <summary>
        /// object[EPage]로 url을 불러 올 수 있음.
        /// </summary>
        internal string this[EPage ePage]
        {
            get
            {
                int index = (int)ePage;
                return pages[index];
            }
        }
        /// <summary>
        /// object[EPost]로 POST Data 값을 받음.
        /// </summary>
        internal string this[EPost ePost]
        {
            get
            {
                int index = (int)ePost;
                return page_post[index];
            }
        }
    }

    /// <summary>
    /// POST DATA의 인덱스를 열거
    /// </summary>
    internal enum EPost
    {
        eMain = 0,
        eWaffleMsg = 1
    }

    /// <summary>
    ///페이지 인덱스를 열거
    /// </summary>
    internal enum EPage
    {
        eMyClass = 0,
        eMain = 0,
        eClasses = 1,
        eAttend = 2,
        eAdditionMovie = 3,
        eAdditionFile = 4,
        eLectureMovie = 5,
        eLectureHwork = 6,
        eReturnJSP = 7
    }
}
