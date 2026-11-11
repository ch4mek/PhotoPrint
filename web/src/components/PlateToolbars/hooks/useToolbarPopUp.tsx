import { MouseEvent, useCallback } from 'react';

import { useToolbarContext } from '@/context/ToolbarContext';
import { EditorToolbarFormValues } from '@/forms/EditorToolbarForm/types';
import { MediaType } from '@/types/plateEditor';

export function useToolbarPopUp() {
	const { setPopUpData, handleOpen, handleClose } = useToolbarContext();

	const handleButtonClick = useCallback(
		(type: MediaType, handleSubmit: (data: EditorToolbarFormValues) => void) =>
			(e: MouseEvent<HTMLSpanElement>) => {
				e.preventDefault();
				setPopUpData({
					type,
					handleSubmit,
				});

				handleOpen();
			},
		[handleOpen]
	);

	return {
		handleButtonClick,
		handleClose,
	};
}
