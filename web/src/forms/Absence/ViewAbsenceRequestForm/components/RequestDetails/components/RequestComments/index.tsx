import { Checkbox } from '@mms/mms-ui-library/components/Checkbox';
import { spaces } from '@mms/mms-ui-library/theme';
import { useCallback } from 'react';

import { FormSection } from '@/components/FormSection';
import { COMMENT_EDITOR_ID } from '@/constants/plateEditor';
import { useToast } from '@/context/Toast';
import { ToolbarProvider } from '@/context/ToolbarContext';
import { getFilesToUpload } from '@/forms/Absence/RequestAbsenceForm/utils/getFilesToUpload';
import { useUploadFiles } from '@/queries/files/useUploadFiles';
import { useCreateRequestComment } from '@/queries/inquiries/comments/useCreateRequestComment';
import { useGetRequestComments } from '@/queries/inquiries/comments/useGetRequestComments';
import { AppFile } from '@/types/file';

import { CommentEditor } from './components/CommentEditor';
import { PlateModes } from './components/CommentEditor/constants';
import { CommentsList } from './components/CommentsList';
import {
	CREATE_BUTTON_LABEL,
	NOTIFICATION_CHECKBOX_TEXT,
	SECTION_TITLE,
} from './constants';
import { useApproversNotificationState } from './hooks/useApproversNotificationState';
import { SendCommentButton } from './styled';
import { RequestCommentsProps } from './types';
import { mapUploadedFilesToAppFiles } from './utils/mapUploadedFilesToAppFiles';

export function RequestComments({ requestId }: RequestCommentsProps) {
	const toast = useToast();

	const { isNotifyApproversChecked, toggleNotifyApproversCheckbox } =
		useApproversNotificationState();

	const { data: comments, isLoading: isLoadingComments } =
		useGetRequestComments(requestId);

	const { mutateAsync: addComment, isPending: isAddingComment } =
		useCreateRequestComment(toast);

	const { mutateAsync: uploadFiles, isPending: isUploadingFiles } =
		useUploadFiles();

	const handleAddComment = useCallback(
		async (commentText: string, files: AppFile[]) => {
			let preparedFiles: AppFile[] = [];

			if (files.length > 0) {
				const filesToUpload = getFilesToUpload(files);

				if (filesToUpload.length > 0) {
					const uploadedFiles = await uploadFiles(filesToUpload);
					preparedFiles = mapUploadedFilesToAppFiles(
						uploadedFiles,
						filesToUpload
					);
				}
			}

			await addComment({
				comment: commentText,
				inquiryId: requestId,
				files: preparedFiles,
				notifyApprovers: isNotifyApproversChecked,
			});
		},
		[uploadFiles, addComment, requestId, isNotifyApproversChecked]
	);

	return (
		<FormSection title={SECTION_TITLE} gap={spaces.s}>
			<ToolbarProvider>
				{!isLoadingComments && comments && (
					<CommentsList comments={comments} requestId={requestId} />
				)}

				<CommentEditor
					onSubmit={handleAddComment}
					isSubmitting={isAddingComment || isUploadingFiles}
					editorId={`${COMMENT_EDITOR_ID}-${requestId}-create`}
					mode={PlateModes.create}
				>
					{({ isSendDisabled }) => (
						<>
							<Checkbox
								label={NOTIFICATION_CHECKBOX_TEXT}
								checked={isNotifyApproversChecked}
								onChange={toggleNotifyApproversCheckbox}
							/>
							<SendCommentButton type="submit" disabled={isSendDisabled}>
								{CREATE_BUTTON_LABEL}
							</SendCommentButton>
						</>
					)}
				</CommentEditor>
			</ToolbarProvider>
		</FormSection>
	);
}
