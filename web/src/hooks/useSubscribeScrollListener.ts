import { useEffect } from 'react';

import { useListScrollObserver } from '@/context/ListScrollObserver';

export const useSubscribeScrollListener = (
	isOpen: boolean,
	onClose: () => void,
	index: number
) => {
	const listScrollObserver = useListScrollObserver();

	useEffect(() => {
		if (isOpen) {
			const handleScroll = (startRowIdx?: number, endRowIdx?: number) => {
				if (!startRowIdx || !endRowIdx) {
					return;
				}

				if (index > endRowIdx || index < startRowIdx) {
					onClose();
				}
			};

			listScrollObserver.addScrollListener(handleScroll);

			return () => listScrollObserver.removeScrollListener(handleScroll);
		}
	}, [isOpen, index, onClose]);
};
