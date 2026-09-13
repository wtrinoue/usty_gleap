public interface StatusRule
{
    void Calculate(in StatusVector source, in StatusVector target);
}

// in修飾子は参照渡しとconstの合体みたいなもの