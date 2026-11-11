import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { GROUP_FORM_POPUP_ENTITY_NAME } from '../constants';

import { confirmationMessage } from '@/components/DialogTypography/constants';
import { ConfirmationTypography } from '@/components/DialogTypography/styles';
import { getDeletingPopUpTitle } from '@/constants/activityStatus';
import { ITEM_WILL_DELETED_POSTFIX } from '@/constants/notifications';
import { useGroupMutations } from '@/pages/AdminPanel/components/DepartmentsTab/hooks/useGroupMutations';
import { useGetGroupById } from '@/queries/departments/useGetGroupById';
import { GroupMutationType } from '@/types/department';

import { GroupDeleteFormProps } from './types';

export function GroupDeleteForm({
	id,
	handleClose,
	refetch,
}: GroupDeleteFormProps) {
	const { data, isLoading, isFetching } = useGetGroupById(id, {
		enabled: Boolean(id),
	});
	const { handleMutation, isMutationLoading } = useGroupMutations(
		GroupMutationType.Delete,
		handleClose,
		refetch
	);

	const showPopupLoader = isMutationLoading || isLoading || isFetching;

	const handleDeleteGroup = () => {
		handleMutation(id);
	};

	return (
		<PopUp
			title={getDeletingPopUpTitle(GROUP_FORM_POPUP_ENTITY_NAME)}
			type="approve"
			showCloseButton={false}
			onClose={handleClose}
			controls={{
				positiveControl: {
					onClick: handleDeleteGroup,
				},
				negativeControl: {
					onClick: handleClose,
				},
			}}
			showLoader={showPopupLoader}
		>
			<Typography variant="m-600">
				{data?.name}
				<Typography variant="m-400"> {ITEM_WILL_DELETED_POSTFIX}</Typography>
			</Typography>
			<ConfirmationTypography variant="m-400">
				{confirmationMessage}
			</ConfirmationTypography>
		</PopUp>
	);
}
