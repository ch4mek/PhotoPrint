import { useTheme } from '@emotion/react';
import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { PlusIcon } from '@mms/mms-ui-library/icons/base/PlusIcon';
import {
	memo,
	UIEvent,
	useCallback,
	useEffect,
	useMemo,
	useRef,
	useState,
} from 'react';

import { TILE_LIST_ITEM_HEIGHT } from '../../constants';
import { TileListItem } from '../TileListItem';

import { Loader } from '@/components/Loader';
import { DEFAULT_ROWS_PER_PAGE_INT } from '@/constants/pagination';
import { useListScrollObserver } from '@/context/ListScrollObserver';
import { useScrollToBottom } from '@/hooks/useScrollToBottom';

import { VIRTUALIZATION_BUFFERED_ITEMS_COUNT } from './constants';
import {
	NoItemsMessageWrapper,
	ScrollbarWrapper,
	TileListBody,
	TileListContent,
	TileListControls,
	TileListHeaderColorWrapper,
	TileListHeaderWrapper,
	TileListWrapper,
	TitleTypography,
	TotalCountContainer,
	TotalTypography,
} from './styles';
import { TileListProps } from './types';

function TileListInner<T extends { id: number | string }>({
	items,
	onAddClick,
	title,
	beforeColor,
	onSelect,
	selectedIndex,
	totalItems,
	showLoader = false,
	isLoading = false,
	isPointerCursor = false,
	page,
	setPage,
	getListItemContextMenuOption,
	fieldToDisplay,
	containerRef,
}: TileListProps<T>) {
	const theme = useTheme();
	const internalScrollWrapperRef = useRef<HTMLDivElement | null>(null);
	const [containerHeight, setContainerHeight] = useState(0);
	const [scrollPosition, setScrollPosition] = useState(0);

	const listScrollObserver = useListScrollObserver();

	const gap = theme.spaces.xs;
	const rowHeight = TILE_LIST_ITEM_HEIGHT + gap;

	const fullHeight = useMemo(
		() => (showLoader ? '100%' : `${items.length * rowHeight - gap}px`),
		[showLoader, items]
	);

	const scrollWrapperRef = containerRef || internalScrollWrapperRef;

	useEffect(() => {
		if (!scrollWrapperRef.current) {
			return;
		}

		const resizeObserver = new ResizeObserver(() => {
			if (scrollWrapperRef.current?.offsetHeight) {
				setContainerHeight(scrollWrapperRef.current.offsetHeight);
			}
		});

		resizeObserver.observe(scrollWrapperRef.current);

		return () => {
			resizeObserver.disconnect();
		};
	}, []);

	const handleAddClick = useCallback(() => {
		if (onAddClick) {
			onAddClick();
		}
	}, [onAddClick]);

	const handleItemClick = useCallback(
		(itemId: T) => {
			if (onSelect) {
				onSelect(itemId);
			}
		},
		[onSelect]
	);

	const handleScrollToBottom = useCallback(() => {
		if (
			page === undefined ||
			!setPage ||
			showLoader ||
			isLoading ||
			totalItems <= (page + 1) * DEFAULT_ROWS_PER_PAGE_INT
		) {
			return;
		}
		setPage(title, page + 1);
	}, [items, page, showLoader, totalItems]);

	const handleListScroll = (event: UIEvent<HTMLDivElement>) => {
		setScrollPosition(event.currentTarget.scrollTop);

		const { scrollTop, clientHeight } = event.target as HTMLElement;

		const startIdx = Math.floor(scrollTop / rowHeight);
		const visibleRowsCount = Math.ceil(clientHeight / rowHeight);
		const endIdx = startIdx + visibleRowsCount - 1;

		listScrollObserver.notifyScrollListeners(startIdx, endIdx);
	};

	useScrollToBottom(scrollWrapperRef.current, handleScrollToBottom);

	const visibleChildren = useMemo(() => {
		const startIndex = Math.max(
			Math.floor(scrollPosition / rowHeight) -
				VIRTUALIZATION_BUFFERED_ITEMS_COUNT,
			0
		);
		const endIndex = Math.min(
			Math.ceil((scrollPosition + containerHeight) / rowHeight - 1) +
				VIRTUALIZATION_BUFFERED_ITEMS_COUNT,
			items.length - 1
		);

		return items.slice(startIndex, endIndex + 1).map((child, index) => ({
			topPosition: (startIndex + index) * rowHeight,
			item: child,
			index: startIndex + index,
		}));
	}, [items, containerHeight, scrollPosition]);

	return (
		<TileListWrapper>
			<TileListHeaderColorWrapper>
				<TileListHeaderWrapper beforeColor={theme.palette[beforeColor]}>
					<TitleTypography variant="s-600">{title}</TitleTypography>
					<TileListControls>
						<TotalCountContainer>
							<TotalTypography variant="s-600">{totalItems}</TotalTypography>
						</TotalCountContainer>
						{onAddClick && (
							<ActionButton onClick={handleAddClick} size="small">
								<PlusIcon />
							</ActionButton>
						)}
					</TileListControls>
				</TileListHeaderWrapper>
			</TileListHeaderColorWrapper>
			<TileListBody containsLoader={showLoader}>
				<ScrollbarWrapper ref={scrollWrapperRef} onScroll={handleListScroll}>
					<TileListContent height={fullHeight}>
						<Loader isLoading={showLoader}>
							{visibleChildren.map(({ topPosition, item, index }) => (
								<TileListItem
									top={topPosition}
									key={item.id}
									item={item}
									onClick={handleItemClick}
									isSelected={item.id === selectedIndex}
									index={index}
									fieldToDisplay={fieldToDisplay}
									getContextMenuOptions={getListItemContextMenuOption}
									isPointerCursor={isPointerCursor}
								/>
							))}
							{!items.length && (
								<NoItemsMessageWrapper variant="m-400" align="center" fullWidth>
									No items
								</NoItemsMessageWrapper>
							)}
						</Loader>
					</TileListContent>
				</ScrollbarWrapper>
			</TileListBody>
		</TileListWrapper>
	);
}

export const TileList = memo(TileListInner) as typeof TileListInner;
