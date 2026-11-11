import { PickerButton } from '@mms/mms-ui-library/components/PickerButton';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { OptionDotsIcon } from '@mms/mms-ui-library/icons/base/OptionDotsIcon';
import { memo, MouseEvent, useCallback, useMemo, useRef } from 'react';

import { optionDotsIconStyles } from './constants';
import { TileListItemWrapper, TileListTypography } from './styles';
import { TileListItemProps } from './types';

function TileListItemInner<T extends { id: number | string }>({
	onClick,
	isSelected,
	getContextMenuOptions,
	item,
	top,
	fieldToDisplay,
	isPointerCursor,
}: TileListItemProps<T>) {
	const itemName = String(item[fieldToDisplay]);
	const pickerButtonRef = useRef<HTMLButtonElement>(null);

	const handleTileItemClick = useCallback(
		(event: MouseEvent<HTMLDivElement>) => {
			if (
				pickerButtonRef.current &&
				pickerButtonRef.current.contains(event.target as Node)
			) {
				return;
			}

			if (onClick) {
				onClick(item);
			}
		},
		[onClick]
	);

	const contextMenuOptions = useMemo(() => {
		if (!getContextMenuOptions) {
			return [];
		}

		return getContextMenuOptions(item);
	}, [getContextMenuOptions, item]);

	const displayContextMenuButton = contextMenuOptions?.length !== 0;

	return (
		<TileListItemWrapper
			onClick={handleTileItemClick}
			isSelected={isSelected}
			top={top}
			sameSidesPaddings={!displayContextMenuButton}
			isPointerCursor={isPointerCursor}
		>
			<Tooltip useIfTextCut text={itemName}>
				<TileListTypography variant="m-400">{itemName}</TileListTypography>
			</Tooltip>
			{displayContextMenuButton && (
				<PickerButton
					ref={pickerButtonRef}
					icon={<OptionDotsIcon {...optionDotsIconStyles} />}
					options={contextMenuOptions}
					item={item}
				/>
			)}
		</TileListItemWrapper>
	);
}

export const TileListItem = memo(TileListItemInner) as typeof TileListItemInner;
