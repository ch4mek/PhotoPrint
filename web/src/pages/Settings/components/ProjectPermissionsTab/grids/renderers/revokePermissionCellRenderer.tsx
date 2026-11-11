import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { RevokeIcon } from '@mms/mms-ui-library/icons/base/RevokeIcon';
import { Skeleton } from '@mui/material';
import { useQueryClient } from '@tanstack/react-query';
import { useMemo, useState } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { BILLABLE_WORKING_HOURS_PERMISSIONS_QUERY_KEY } from '../../components/BillableWorkingHoursPermissionsGrid/hooks/useBillableWorkingHoursPermissions';
import { BillableWorkingHoursPermissionsRow } from '../../components/BillableWorkingHoursPermissionsGrid/hooks/useBillableWorkingHoursPermissionsRows/types';
import { useRevokePermissionAssignment } from '../../components/BillableWorkingHoursPermissionsGrid/hooks/useRevokePermissionAssignment';
import { PERMISSION_REVOKING_POPUP_TITLE } from '../../constants';
import { getPermissionAssignmentToastMessage } from '../../utils';

import { NotificationType } from '@/constants/notifications';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { revokingPermissionsPopupRenderer } from '@/pages/Settings/components/ProjectPermissionsTab/renderers/revokingPermissionsPopupRenderer';

export const revokePermissionCellRenderer = <
	T extends BillableWorkingHoursPermissionsRow
>({
	row: { assignmentId, userFullName, showSkeleton, projectName },
}: RenderCellProps<T, unknown>) => {
	const [isOpenConfirmationDialog, setIsOpenConfirmationDialog] =
		useState(false);

	const queryClient = useQueryClient();
	const toast = useToast();

	const handleCloseConfirmationDialog = () => {
		setIsOpenConfirmationDialog(false);
	};

	const handleOpenConfirmationDialog = () => {
		setIsOpenConfirmationDialog(true);
	};

	const { mutate } = useRevokePermissionAssignment({
		onSuccess() {
			queryClient.invalidateQueries({
				queryKey: [BILLABLE_WORKING_HOURS_PERMISSIONS_QUERY_KEY],
			});

			toast.open(
				createToast(
					NotificationType.SUCCESS,
					<>
						{getPermissionAssignmentToastMessage()}
						<Typography variant="m-600">{userFullName}</Typography>
					</>
				)
			);
		},
	});

	const handleApplyConfirmationDialog = () => {
		mutate({
			assignmentId,
		});
		setIsOpenConfirmationDialog(false);
	};

	const RevokePopupContent = useMemo(
		() => revokingPermissionsPopupRenderer(userFullName),
		[userFullName]
	);

	return showSkeleton ? (
		<Skeleton animation="wave" width="100%" />
	) : (
		<>
			<ActionButton onClick={handleOpenConfirmationDialog} size="small">
				<RevokeIcon />
			</ActionButton>
			{isOpenConfirmationDialog && (
				<PopUp
					title={PERMISSION_REVOKING_POPUP_TITLE}
					onClose={handleCloseConfirmationDialog}
					type="approve"
					showCloseButton={false}
					controls={{
						negativeControl: { onClick: handleCloseConfirmationDialog },
						positiveControl: {
							onClick: handleApplyConfirmationDialog,
						},
					}}
				>
					<RevokePopupContent>{projectName}</RevokePopupContent>
				</PopUp>
			)}
		</>
	);
};
