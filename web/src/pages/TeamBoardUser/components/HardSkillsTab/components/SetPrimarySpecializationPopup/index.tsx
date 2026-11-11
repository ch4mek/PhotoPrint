import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { zIndexes } from '@/constants/zIndexes';
import {
	SET_PRIMARY_POPUP_TITLE,
	SET_PRIMARY_SPECIALIZATION_MESSAGE,
	SET_PRIMARY_SPECIALIZATION_QUESTION,
} from '@/pages/TeamBoardUser/components/HardSkillsTab/constants';

import { SetPrimaryQuestionTypography } from './styles';
import { SetPrimarySpecializationPopupProps } from './types';

export function SetPrimarySpecializationPopup({
	nameOfSpecializationPrimary,
	handleSetPrimarySpecialization,
	handleCloseSetPrimaryDialog,
}: SetPrimarySpecializationPopupProps) {
	return (
		<PopUp
			title={SET_PRIMARY_POPUP_TITLE}
			onClose={handleCloseSetPrimaryDialog}
			type="approve"
			showCloseButton={false}
			controls={{
				positiveControl: { onClick: handleSetPrimarySpecialization },
				negativeControl: { onClick: handleCloseSetPrimaryDialog },
			}}
			zIndex={zIndexes.formPopup}
		>
			<Typography variant="m-400">
				<Typography variant="m-600">{nameOfSpecializationPrimary}</Typography>
				{SET_PRIMARY_SPECIALIZATION_MESSAGE}
			</Typography>
			<SetPrimaryQuestionTypography variant="m-400">
				{SET_PRIMARY_SPECIALIZATION_QUESTION}
			</SetPrimaryQuestionTypography>
		</PopUp>
	);
}
