import { useCallback, useState } from 'react';

export const useOpenCollapsibleRow = () => {
	const [isRowOpen, setIsRowOpen] = useState(false);

	const handleSetRowOpen = useCallback(() => {
		setIsRowOpen((prevOpenState) => !prevOpenState);
	}, []);

	return { isRowOpen, handleSetRowOpen };
};
