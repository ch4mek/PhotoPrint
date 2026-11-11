import { Comment } from '@/types/comments';
import { CustomValue } from '@/types/plateEditor';

export interface CommentViewerProps {
	commentInfo: Comment;
	value: CustomValue;
	inquiryId: number;
}
