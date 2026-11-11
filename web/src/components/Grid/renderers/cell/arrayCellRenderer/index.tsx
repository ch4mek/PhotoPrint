import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { useOutsideClickMany } from '@mms/mms-ui-library/hooks/useOutsideClickMany';
import { Skeleton } from '@mui/material';
import { useRef, useState, useCallback, MouseEvent } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { GridCellDefaultProps } from '../../types';

import { ChevronButton } from '@/components/ChevronButton';
import { zIndexes } from '@/constants/zIndexes';
import { useSubscribeScrollListener } from '@/hooks/useSubscribeScrollListener';

import { DropDown } from './DropDown';
import {
	ArrayCellWrapper,
	ArrayDropDownWrapper,
	ChevronWrapper,
	FirstItem,
	FirstItemWrapper,
} from './styles';

export const ArrayCellRenderer = <T extends GridCellDefaultProps>(
	{ row, column: { maxWidth, key }, ...restProps }: RenderCellProps<T, unknown>,
	includeFirstItemInDropDown = true
) => {
	const [isOpen, setIsOpen] = useState(false);
	const wrapperRef = useRef(null);
	const currentRowIdx = restProps.rowIdx;
	const chevronWrapperRef = useRef(null);

	const array = row[key as keyof T] as string[];

	const { showSkeleton, disabled } = row;

	const handleClickOutside = useCallback(() => setIsOpen(false), []);
	useOutsideClickMany([wrapperRef], handleClickOutside);

	const handleClick = () => {
		setIsOpen((p) => !p);
	};

	const handleClose = useCallback(() => setIsOpen(false), []);

	useSubscribeScrollListener(isOpen, handleClose, currentRowIdx);

	const handleOnMouseDownWrapper = (e: MouseEvent<HTMLDivElement>) => {
		e.preventDefault();
	};

	return showSkeleton ? (
		<Skeleton animation="wave" width={maxWidth} />
	) : (
		<>
			<ArrayCellWrapper disabled={disabled} ref={wrapperRef}>
				{array.length <= 1 ? (
					<Tooltip
						zIndex={zIndexes.gridTooltip}
						text={array[0]}
						position="bottom"
						fallbackPlacements={['bottom']}
						useIfTextCut
					>
						<FirstItem variant="m-400">{array[0]}</FirstItem>
					</Tooltip>
				) : (
					<ArrayDropDownWrapper onMouseDown={handleOnMouseDownWrapper}>
						<FirstItemWrapper onClick={handleClick}>
							<Tooltip
								zIndex={zIndexes.gridTooltip}
								text={array[0]}
								position="bottom"
								fallbackPlacements={['bottom']}
								useIfTextCut
							>
								<FirstItem variant="m-400">{array[0]}</FirstItem>
							</Tooltip>
							<ChevronWrapper ref={chevronWrapperRef}>
								<ChevronButton isActive={isOpen} />
							</ChevronWrapper>
						</FirstItemWrapper>
					</ArrayDropDownWrapper>
				)}
			</ArrayCellWrapper>
			{isOpen && array.length > 1 && (
				<DropDown
					isOpen={isOpen}
					anchorElement={chevronWrapperRef.current}
					items={includeFirstItemInDropDown ? array : array.slice(1)}
				/>
			)}
		</>
	);
};
