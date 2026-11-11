import { BasicCrossIcon } from '@mms/mms-ui-library/icons/base/BasicCrossIcon';
import { BurgerMenuIcon } from '@mms/mms-ui-library/icons/base/BurgerMenuIcon';

import { ControlButtonWrapper } from '@/components/MobileSidebar/components/ControlButton/styles';
import { ControlButtonProps } from '@/components/MobileSidebar/components/ControlButton/types';

export function ControlButton({ isOpen, onClick }: ControlButtonProps) {
	return (
		<ControlButtonWrapper onClick={onClick}>
			{isOpen ? <BasicCrossIcon /> : <BurgerMenuIcon />}
		</ControlButtonWrapper>
	);
}
