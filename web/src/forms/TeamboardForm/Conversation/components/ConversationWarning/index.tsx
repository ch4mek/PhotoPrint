import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { shortConfirmationMessage } from '@/components/DialogTypography/constants';
import { ConfirmationTypography } from '@/components/DialogTypography/styles';
import { isGroupManager } from '@/constants/roles';
import { zIndexes } from '@/constants/zIndexes';
import { useAuth } from '@/context/AuthContext';
import {
	WARNING_POPUP_MESSAGE,
	WARNING_POPUP_MESSAGE_WITHOUT_SALARY,
} from '@/forms/TeamboardForm/Conversation/components/ConversationMessages';
import { ConversationWarningProps } from '@/forms/TeamboardForm/Conversation/components/ConversationWarning/types';

import { POP_UP_TITLE } from './constants';

export function ConversationWarning({
	handleToggleDialog,
	handleCloseAllPopUps,
}: ConversationWarningProps) {
	const { role } = useAuth();
	const isGroupMenagerRole = isGroupManager(role);

	return (
		<PopUp
			title={POP_UP_TITLE}
			onClose={handleToggleDialog}
			type="approve"
			showCloseButton={false}
			controls={{
				positiveControl: { onClick: handleCloseAllPopUps },
				negativeControl: { onClick: handleToggleDialog },
			}}
			zIndex={zIndexes.formPopup}
		>
			<Typography variant="m-400">
				{isGroupMenagerRole
					? WARNING_POPUP_MESSAGE_WITHOUT_SALARY
					: WARNING_POPUP_MESSAGE}
			</Typography>
			<ConfirmationTypography variant="m-400">
				{shortConfirmationMessage}
			</ConfirmationTypography>
		</PopUp>
	);
}
