import { Comment } from '@/types/comments';

export interface CommentsListProps {
	comments: Comment[];
	requestId: number;
}
