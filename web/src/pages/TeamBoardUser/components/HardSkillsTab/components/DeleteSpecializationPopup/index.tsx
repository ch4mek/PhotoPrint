import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { zIndexes } from '@/constants/zIndexes';
import { DeleteSpecializationPopupProps } from '@/pages/TeamBoardUser/components/HardSkillsTab/components/DeleteSpecializationPopup/types';
import {
	DELETE_SPECIALIZATION_MESSAGE_END,
	DELETE_SPECIALIZATION_MESSAGE_START,
	DELETING_POPUP_TITLE,
} from '@/pages/TeamBoardUser/components/HardSkillsTab/constants';

export function DeleteSpecializationPopup({
	nameOfSpecializationToDelete,
	handleDeleteSpecialization,
	handleCloseDeleteDialog,
}: DeleteSpecializationPopupProps) {
	return (
		<PopUp
			title={DELETING_POPUP_TITLE}
			onClose={handleCloseDeleteDialog}
			type="approve"
			showCloseButton={false}
			controls={{
				positiveControl: { onClick: handleDeleteSpecialization },
				negativeControl: { onClick: handleCloseDeleteDialog },
			}}
			zIndex={zIndexes.formPopup}
		>
			<Typography variant="m-400">
				{DELETE_SPECIALIZATION_MESSAGE_START}
				<Typography variant="m-600">{nameOfSpecializationToDelete}</Typography>
				{DELETE_SPECIALIZATION_MESSAGE_END}
			</Typography>
		</PopUp>
	);
}
