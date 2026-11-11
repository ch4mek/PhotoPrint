import { useMemo } from 'react';

import { CommentItem } from '../CommentItem';

import { initialEditorData } from '@/constants/plateEditor';

import { CommentsListProps } from './types';

export function CommentsList({ comments, requestId }: CommentsListProps) {
	const parsedComments = useMemo(
		() =>
			comments?.map((comment) => {
				const parsedEditorData = comment.comment
					? JSON.parse(comment.comment)
					: initialEditorData;

				return { ...comment, parsedEditorData };
			}) || [],
		[comments]
	);

	return parsedComments?.map((comment) => (
		<CommentItem
			key={comment.id}
			commentInfo={comment}
			value={comment.parsedEditorData}
			inquiryId={requestId}
		/>
	));
}
