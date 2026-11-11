import { useMemo } from 'react';

import { controlsText } from '@/constants/controlsTexts';
import { checkIsDocumentRequest } from '@/helpers/absences';
import { ManageRequestType } from '@/types/absences';

import { ControlButton, RequestControlsWrapper } from './styles';
import { RequestControlsProps } from './types';

const { approveText, doneText, rejectText } = controlsText;
const { Accept, Reject } = ManageRequestType;

export function RequestControls({
	handleOpen,
	inquiryType,
	canCurrentUserApprove = false,
	canCurrentUserReject = false,
}: RequestControlsProps) {
	const handleManageRequest = (type: ManageRequestType) => () => {
		handleOpen(type);
	};

	const positiveControlText = useMemo(
		() => (checkIsDocumentRequest(inquiryType) ? doneText : approveText),
		[inquiryType]
	);

	return (
		<RequestControlsWrapper>
			{canCurrentUserReject && (
				<ControlButton
					isFullWidth={false}
					color="secondary"
					onClick={handleManageRequest(Reject)}
				>
					{rejectText}
				</ControlButton>
			)}
			{canCurrentUserApprove && (
				<ControlButton
					isFullWidth={false}
					onClick={handleManageRequest(Accept)}
				>
					{positiveControlText}
				</ControlButton>
			)}
		</RequestControlsWrapper>
	);
}
