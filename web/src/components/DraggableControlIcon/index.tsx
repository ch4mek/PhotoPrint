import { useTheme } from '@emotion/react';
import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { PickerButton } from '@mms/mms-ui-library/components/PickerButton';
import { DotsMoveIcon } from '@mms/mms-ui-library/icons/base/DotsMoveIcon';
import { OptionDotsIcon } from '@mms/mms-ui-library/icons/base/OptionDotsIcon';

import { DraggableControlIconWrapper } from '@/components/DraggableControlIcon/styles';
import { DraggableControlIconProps } from '@/components/DraggableControlIcon/types';

export function DraggableControlIcon({
	menuOptions,
	isMoveMode,
	item,
	withIconSpace = true,
	iconSize = 'small',
	isDisablePickerButton = true,
}: DraggableControlIconProps) {
	const theme = useTheme();

	return (
		<DraggableControlIconWrapper
			withIconSpace={withIconSpace}
			iconSize={iconSize}
		>
			{isMoveMode ? (
				<ActionButton>
					<DotsMoveIcon color={theme.palette['graphics-input-stroke']} />
				</ActionButton>
			) : (
				isDisablePickerButton && (
					<PickerButton
						icon={<OptionDotsIcon />}
						options={menuOptions || []}
						popoverPosition="left-start"
						item={item}
					/>
				)
			)}
		</DraggableControlIconWrapper>
	);
}
