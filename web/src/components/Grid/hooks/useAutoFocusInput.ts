import { useEffect, useRef } from 'react';

export const useAutoFocusInput = (isOpen: boolean) => {
	const ref = useRef<HTMLInputElement>(null);

	useEffect(() => {
		if (!isOpen || !ref.current) {
			return;
		}

		const rafId = requestAnimationFrame(() => {
			ref.current?.focus();
		});

		return () => cancelAnimationFrame(rafId);
	}, [isOpen]);

	return ref;
};
