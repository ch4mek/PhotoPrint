import { useTheme } from '@emotion/react';
import { useMemo } from 'react';

import { UserCommentCard } from '@/components/UserCommentCard';
import { COMMENT_EDITOR_ID } from '@/constants/plateEditor';
import { getCustomPluginsWithThemeMode } from '@/plugins/PlateEditorPlugins';

import { CommentContent } from './components/CommentContent';
import { CommentWrapper } from './styled';
import { CommentViewerProps } from './types';

export function CommentItem({
	commentInfo,
	value,
	inquiryId,
}: CommentViewerProps) {
	const theme = useTheme();

	const plugins = getCustomPluginsWithThemeMode(theme.mode);

	const { id: commentId, files, createdAtUtc, createdByUser } = commentInfo;

	const editorId = useMemo(
		() => `${COMMENT_EDITOR_ID}_${inquiryId}_${commentId}_view`,
		[inquiryId, commentId]
	);

	return (
		<CommentWrapper>
			<UserCommentCard {...{ createdAtUtc, createdByUser }} />

			<CommentContent
				key={`view-${commentId}`}
				{...{
					value,
					plugins,
					files,
					editorId,
				}}
			/>
		</CommentWrapper>
	);
}
