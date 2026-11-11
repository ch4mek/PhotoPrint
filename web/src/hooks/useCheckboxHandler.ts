import { Dispatch, SetStateAction } from 'react';

export const useCheckboxHandler = (
	selection: string[],
	setSelection: Dispatch<SetStateAction<string[]>>
) => {
	const handleCheckboxClick = (id: string) => () => {
		if (!selection) {
			return setSelection([id]);
		}

		if (selection.includes(id)) {
			return setSelection(selection.filter((filt: string) => filt !== id));
		}

		return setSelection((prev: string[]) => [...prev, id]);
	};

	return handleCheckboxClick;
};
