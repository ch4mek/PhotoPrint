import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { RevokeIcon } from '@mms/mms-ui-library/icons/base/RevokeIcon';
import { useQueryClient } from '@tanstack/react-query';
import { useCallback, useContext, useState } from 'react';

import { GroupInfoContext } from '../../context/GroupInfoContext';
import {
	REVOKE_POPUP_LABEL,
	REVOKE_WARNING_TEXT,
	USER_REVOKED_TOAST_CONTENT,
} from '../constants';
import { GroupMembersRow } from '../types';

import { confirmationMessage } from '@/components/DialogTypography/constants';
import { NotificationType } from '@/constants/notifications';
import { GroupQueriesKeys, UsersQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useRevokeGroupMember } from '@/queries/groups/useRevokeGroupMember';

export const RevokeGroupMemberRenderer = ({
	id,
	fullName,
}: GroupMembersRow) => {
	const [isRevokingGroupMember, setIsRevokingGroupMember] = useState(false);
	const { groupId } = useContext(GroupInfoContext);
	const toast = useToast();
	const queryClient = useQueryClient();

	const { mutateAsync, isPending } = useRevokeGroupMember({
		groupId,
		options: {
			onError: createOnErrorHandler(toast),
			onSuccess: () => {
				queryClient.invalidateQueries({
					queryKey: [GroupQueriesKeys.groupMembers],
				});
				queryClient.invalidateQueries({
					queryKey: [GroupQueriesKeys.departmentUsersWithoutGroup],
				});
				queryClient.invalidateQueries({ queryKey: [UsersQueriesKeys.users] });
			},
		},
	});

	const revokeGroupMember = useCallback(async () => {
		await mutateAsync(id);
		setIsRevokingGroupMember(false);
		toast.open(
			createToast(
				NotificationType.SUCCESS,
				<Typography variant="m-400">
					<Typography variant="m-600">{fullName}</Typography>{' '}
					{USER_REVOKED_TOAST_CONTENT}
				</Typography>
			)
		);
	}, [id, mutateAsync]);

	const handleRevokeButtonClick = () => setIsRevokingGroupMember(true);

	const handleRevokeGroupMemberCancel = () => setIsRevokingGroupMember(false);

	return (
		<>
			<ActionButton onClick={handleRevokeButtonClick}>
				<RevokeIcon />
			</ActionButton>
			{isRevokingGroupMember && (
				<PopUp
					title={REVOKE_POPUP_LABEL}
					onClose={handleRevokeGroupMemberCancel}
					controls={{
						positiveControl: { onClick: revokeGroupMember },
						negativeControl: { onClick: handleRevokeGroupMemberCancel },
					}}
					showLoader={isPending}
				>
					<Typography variant="m-400">
						<Typography variant="m-600">{fullName}</Typography>
						{REVOKE_WARNING_TEXT}
					</Typography>
					<Typography variant="m-400">{confirmationMessage}</Typography>
				</PopUp>
			)}
		</>
	);
};
