using System.Collections.Generic;

public static class GameData
{
    private static List<Chapter> ChaptersList;
    private static string CurrentChapterID;
    private static int CurrentChapterIndex;
    private static List<Question> Questions;

    public static void SetChapters(List<Chapter> chapters)
    {
        ChaptersList = chapters;
    }

    public static List<Chapter> GetChapters()
    {
        return ChaptersList;
    }

    public static void SetCurrentChapterID(string currentChapterID)
    {
        CurrentChapterID = currentChapterID;
    }

    public static string GetCurrentChapterID()
    {
        return CurrentChapterID;
    }

    public static void SetCurrentChapterIndex(int currentChapterIndex)
    {
        CurrentChapterIndex = currentChapterIndex;
    }
    
    public static int GetCurrentChapterIndex()
    {
        return CurrentChapterIndex;
    }

    public static Chapter GetDataWithChapterID(string chapterID)
    {
        foreach (var chapter in ChaptersList)
        {
            if (chapterID == chapter.chapterID)
                return chapter;
        }

        return null;
    }

    public static void SetQuestions(List<Question> questions)
    {
        Questions = questions;
    }

    public static List<Question> GetQuestions()
    {
        return Questions;
    }

    public static Question GetQuestionById(string questionId)
    {
        foreach (var question in Questions)
        {
            if (questionId == question.id)
            {
                return question;
            }
        }

        return null;
    }
}
