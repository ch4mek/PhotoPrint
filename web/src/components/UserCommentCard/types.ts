import { ReactNode } from 'react';

import { CommentUser } from '@/types/comments';

type CreatedByUserCommentCard = Pick<
	CommentUser,
	'firstName' | 'lastName' | 'avatarFieldId' | 'color'
>;

export interface UserCommentCardProps {
	createdByUser: CreatedByUserCommentCard;
	createdAtUtc: string;
	timeSuffix?: ReactNode;
}
