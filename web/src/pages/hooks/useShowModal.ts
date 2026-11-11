import { SyntheticEvent, useCallback, useState } from 'react';

import type { ShowModal } from './types';

export const useShowModal = <IdT = number | string>() => {
	const [{ isOpen, id }, setDialogState] = useState<ShowModal<IdT>>({
		isOpen: false,
		id: undefined,
	});

	const handleOpen = useCallback((itemId?: IdT) => {
		setDialogState({
			isOpen: true,
			id: itemId ?? undefined,
		});
	}, []);

	const handleClose = useCallback(
		() =>
			setDialogState({
				id: undefined,
				isOpen: false,
			}),
		[]
	);

	const updateRecord = useCallback(
		(_event?: SyntheticEvent<Element, Event>, itemId?: IdT) => {
			setDialogState({
				isOpen: true,
				id: itemId,
			});
		},
		[]
	);

	return { isOpen, id, handleOpen, handleClose, updateRecord };
};
