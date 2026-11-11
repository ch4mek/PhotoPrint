import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useState } from 'react';

import { zIndexes } from '@/constants/zIndexes';

import { POP_UP_TITLE, UNSAVED_DATA_MESSAGE } from './constants';
import { FormWithSaveExitProps } from './types';

export function FormWithSaveExit({
	isFormDirty,
	children,
	onClose,
	title = POP_UP_TITLE,
	message = UNSAVED_DATA_MESSAGE,
	confirmText,
	cancelText,
	zIndex = zIndexes.confirmationPopup,
}: FormWithSaveExitProps) {
	const [showDiscardWarning, setShowDiscardWarning] = useState(false);

	const handleOutsideCloseForm = () => {
		if (isFormDirty) {
			setShowDiscardWarning(true);
		} else {
			onClose();
		}
	};

	const handleStay = () => {
		setShowDiscardWarning(false);
	};

	const handleDiscardAndClose = () => {
		setShowDiscardWarning(false);
		onClose();
	};

	return (
		<>
			{children(handleOutsideCloseForm)}
			{showDiscardWarning && (
				<PopUp
					title={title}
					showCloseButton={false}
					zIndex={zIndex}
					controls={{
						positiveControl: {
							text: confirmText,
							onClick: handleDiscardAndClose,
						},
						negativeControl: {
							text: cancelText,
							onClick: handleStay,
						},
					}}
					onClose={handleStay}
				>
					{message}
				</PopUp>
			)}
		</>
	);
}
