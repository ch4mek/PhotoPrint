import { useState, useRef, useEffect } from 'react';

export const useToggleContent = (initialState = true) => {
	const [isOpened, setIsOpened] = useState(initialState);
	const [contentHeight, setContentHeight] = useState(0);
	const contentRef = useRef<HTMLDivElement>(null);

	const toggleOpen = () => setIsOpened((prev) => !prev);

	useEffect(() => {
		if (!isOpened) {
			setContentHeight(0);

			return;
		}

		if (!contentRef.current) {
			return;
		}

		const observer = new ResizeObserver(() => {
			if (contentRef.current) {
				setContentHeight(contentRef.current.scrollHeight);
			}
		});

		observer.observe(contentRef.current);
		setContentHeight(contentRef.current.scrollHeight);

		return () => observer.disconnect();
	}, [isOpened]);

	return {
		isOpened,
		toggleOpen,
		contentRef,
		contentHeight,
		setIsOpened,
	};
};
