import { useEffect } from 'react';

type KeyPressCallback = VoidFunction | ((event: KeyboardEvent) => void);

export const useKeyPress = (targetKey: string, onClick: KeyPressCallback) => {
	useEffect(() => {
		const handleKeyPress = (event: KeyboardEvent) => {
			if (event.key === targetKey) {
				onClick(event as never);
			}
		};

		window.addEventListener('keydown', handleKeyPress);

		return () => {
			window.removeEventListener('keydown', handleKeyPress);
		};
	}, [targetKey, onClick]);
};
