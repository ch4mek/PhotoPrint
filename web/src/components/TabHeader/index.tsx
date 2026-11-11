import { ButtonAdd } from '@/components/ButtonAdd';

import { TabHeaderWrapper } from './styles';
import { TabHeaderProps } from './types';

export function TabHeader({
	onAddClick,
	isButtonDisabled = false,
	buttonTitle,
}: TabHeaderProps) {
	return (
		<TabHeaderWrapper>
			<ButtonAdd
				onAdd={onAddClick}
				showTooltip={false}
				disabled={isButtonDisabled}
				title={buttonTitle}
			/>
		</TabHeaderWrapper>
	);
}
