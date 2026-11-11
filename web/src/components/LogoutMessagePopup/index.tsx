import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { zIndexes } from '@/constants/zIndexes';

import { POPUP_OK_TEXT, POPUP_TITLE } from './constants';
import { LogoutMessagePopUpProps } from './types';

export function LogoutMessagePopUp({
	handleClose,
	message,
}: LogoutMessagePopUpProps) {
	return (
		<PopUp
			title={POPUP_TITLE}
			onClose={handleClose}
			type="approve"
			headerType="warning"
			isCloseOnShadow={false}
			showCloseButton={false}
			controls={{
				positiveControl: {
					onClick: handleClose,
					text: POPUP_OK_TEXT,
				},
			}}
			zIndex={zIndexes.logOutPopUp}
		>
			<Typography variant="m-400">{message}</Typography>
		</PopUp>
	);
}
