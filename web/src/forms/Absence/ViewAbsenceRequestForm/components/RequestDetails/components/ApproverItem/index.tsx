import { useTheme } from '@emotion/react';
import { Button } from '@mms/mms-ui-library/components/Button';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { UserCard } from '@/components/UserCard';
import {
	NotificationType,
	ABSENCE_REQUEST_NOTIFICATION_SENT,
} from '@/constants/notifications';
import { useAuth } from '@/context/AuthContext';
import { useToast } from '@/context/Toast';
import { convertDataToUserCard } from '@/forms/Absence/ViewAbsenceRequestForm/utils/convertDataToUserCard';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useNotifyApprover } from '@/queries/inquiries/useNotifyApprover';
import { UserInquiryStatusByApprover } from '@/types/absences';

import {
	NOTIFY_BUTTON_TITLE,
	CONFIRMATION_DIALOG_TITLE,
	CONFIRMATION_DIALOG_BODY,
} from './constants';
import { ApproverInfo, ApproverItemWrapper, IconWrapper } from './styles';
import { ApproverItemProps } from './types';
import { getApproverStatus } from './utils/getApproverStatus';
import { getTooltipLabel } from './utils/getTooltipLabel';

const { Pending, WaitingForApprove } = UserInquiryStatusByApprover;

export function ApproverItem({
	item,
	isDisabled,
	status,
	approver,
	acceptUserPosition,
	acceptPolicyScope,
	inquiryId,
	inquiryAcceptId,
}: ApproverItemProps) {
	const theme = useTheme();
	const toast = useToast();
	const { id: currentUserId } = useAuth();
	const { isOpen, handleOpen, handleClose } = useShowModal();

	const { mutateAsync: notifyApprover, isPending } = useNotifyApprover({
		onSuccess: () => {
			toast.open(
				createToast(NotificationType.SUCCESS, ABSENCE_REQUEST_NOTIFICATION_SENT)
			);
			handleClose();
		},
		onError: createOnErrorHandler(toast),
	});

	const approverInfo = convertDataToUserCard(
		item,
		currentUserId,
		acceptUserPosition,
		acceptPolicyScope
	);
	const { icon, color } = getApproverStatus(theme)[status];

	const handleOpenDialog = () => {
		handleOpen();
	};

	const handleNotify = () => {
		notifyApprover({ inquiryId, inquiryAcceptId });
	};

	return (
		<ApproverItemWrapper>
			<ApproverInfo>
				<Tooltip
					open={!!approver}
					text={getTooltipLabel(
						status,
						approver?.firstName,
						approver?.lastName
					)}
				>
					<IconWrapper {...{ color, isDisabled }}>{icon}</IconWrapper>
				</Tooltip>
				{approverInfo && (
					<UserCard
						{...approverInfo}
						isProfileLinkEnabled={approverInfo.showProfileLink}
					/>
				)}
			</ApproverInfo>
			{(status === Pending || status === WaitingForApprove) && (
				<Button
					color="secondary"
					onClick={handleOpenDialog}
					isFullWidth={false}
				>
					{NOTIFY_BUTTON_TITLE}
				</Button>
			)}
			{isOpen && (
				<PopUp
					title={CONFIRMATION_DIALOG_TITLE}
					showCloseButton={false}
					onClose={handleClose}
					showLoader={isPending}
					controls={{
						positiveControl: { onClick: handleNotify },
						negativeControl: { onClick: handleClose },
					}}
				>
					<Typography variant="m-400">{CONFIRMATION_DIALOG_BODY}</Typography>
				</PopUp>
			)}
		</ApproverItemWrapper>
	);
}
