import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { DELETE_POPUP_CONTENT, DELETE_POPUP_TITLE } from './constants';
import { DeleteProfileAvatarProps } from './types';

export function DeleteProfileAvatar({
	handleClose,
	handleSubmit,
}: DeleteProfileAvatarProps) {
	return (
		<PopUp
			title={DELETE_POPUP_TITLE}
			showCloseButton={false}
			onClose={handleClose}
			controls={{
				positiveControl: { onClick: handleSubmit },
				negativeControl: { onClick: handleClose },
			}}
			type="delete"
		>
			<Typography variant="m-400">{DELETE_POPUP_CONTENT}</Typography>
		</PopUp>
	);
}
