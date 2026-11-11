import { MouseEvent, PropsWithChildren } from 'react';

import { ControlButton } from '@/components/MobileSidebar/components/ControlButton';
import { MobileSidebarProps } from '@/components/MobileSidebar/components/types';
import {
	ControlWrapper,
	LayoutWrapper,
	MobileSidebarWrapper,
} from '@/components/MobileSidebar/styles';

export function MobileSidebar({
	children,
	handleControlSideBar,
	isOpen,
}: PropsWithChildren<MobileSidebarProps>) {
	const handleStopPropagation = (event: MouseEvent<HTMLDivElement>) => {
		event.stopPropagation();
	};

	const handleLayoutClick = (event: MouseEvent<HTMLDivElement>) => {
		handleStopPropagation(event);

		handleControlSideBar();
	};

	return isOpen ? (
		<LayoutWrapper activeShadow={isOpen} onClick={handleLayoutClick}>
			<MobileSidebarWrapper onClick={handleStopPropagation}>
				<ControlWrapper>
					<ControlButton onClick={handleControlSideBar} isOpen={isOpen} />
				</ControlWrapper>
				{children}
			</MobileSidebarWrapper>
		</LayoutWrapper>
	) : null;
}
