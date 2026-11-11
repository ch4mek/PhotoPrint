import { useRef, useEffect, useState } from 'react';

import { ChildrenWrapper } from './styles';
import { CollapseProps } from './types';

export function Collapse({ isOpen = false, children, ...rest }: CollapseProps) {
	const [prevIsOpen, setPrevIsOpen] = useState(isOpen);
	const childrenWrapperRef = useRef<HTMLDivElement>(null);

	useEffect(() => {
		const childrenWrapper = childrenWrapperRef.current;

		if (childrenWrapper && prevIsOpen !== isOpen) {
			childrenWrapper.style.height = `${childrenWrapper.scrollHeight}px`;

			if (!isOpen) {
				setTimeout(() => {
					childrenWrapper.style.height = '';
				}, 0);
			}
			setPrevIsOpen(isOpen);
		}
	}, [isOpen]);

	const handleTransitionEnd = () => {
		const childrenWrapper = childrenWrapperRef.current;

		if (childrenWrapper && isOpen) {
			childrenWrapper.style.height = 'auto';
		}
	};

	return (
		<ChildrenWrapper
			ref={childrenWrapperRef}
			onTransitionEnd={handleTransitionEnd}
			{...rest}
		>
			{children}
		</ChildrenWrapper>
	);
}
