import { useTheme } from '@emotion/react';
import { FilterButton } from '@mms/mms-ui-library/components/FilterButton';
import { Popover } from '@mms/mms-ui-library/components/Popover';
import { useOutsideClickMany } from '@mms/mms-ui-library/hooks/useOutsideClickMany';
import {
	Dispatch,
	JSX,
	MouseEvent,
	ReactNode,
	RefObject,
	SetStateAction,
	SyntheticEvent,
	useCallback,
	useEffect,
	useLayoutEffect,
	useMemo,
	useRef,
	useState,
	useTransition,
} from 'react';
import { RenderHeaderCellProps } from 'react-data-grid';

import { GridCellDefaultProps } from '../../types';
import { RenderModalProps, SelectionModalOption } from '../types';

import { headerCellRenderer } from '@/components/Grid';
import {
	MODAL_WIDTH,
	TOOLTIP_FALLBACK_PLACEMENTS,
} from '@/components/Grid/constants';
import { getPortalContainer } from '@/components/Grid/utils/getPortalContainer';
import { BorderRadiuses } from '@/constants/borderRadiuses';
import { EVENT_ENTER_KEY } from '@/constants/eventKeys';
import { zIndexes } from '@/constants/zIndexes';

import {
	FilterControlWrapper,
	HeaderCellWithFilterContentWrapper,
} from './styles';

interface RenderHeaderProps<Type, Second, FilterType>
	extends RenderHeaderCellProps<Type, Second> {
	defaultValue: FilterType;
	filter: FilterType;
	setFilter: Dispatch<SetStateAction<FilterType>>;
	isFilterApplied?: boolean;
	areValuesEquals: (value1: FilterType, value2: FilterType) => boolean;
	options?: Array<SelectionModalOption>;
	autocompleteOptions?: string[];
	disableFilterIconActiveStateAsDefault?: boolean;
	renderModal?: (props: RenderModalProps<FilterType>) => JSX.Element;
	isApplyButtonDisabled?: (
		prevSelection: FilterType,
		currentSelection: FilterType
	) => boolean;
	allowEmptySelection?: boolean;
	disabledAllSelection?: boolean;
	// Notice: for correct work, container should have style position: relative
	modalContainerSelector?: string;
	calculateAvailableMaxHeight?: boolean;
	displayFilterButton?: boolean;
	tooltipContent?: ReactNode | undefined;
}

const MODAL_AVAILABLE_HEIGHT_OFFSET = 24;

export function HeaderCellWithFilterRenderer<
	T extends GridCellDefaultProps,
	FilterType
>(props: RenderHeaderProps<T, unknown, FilterType>) {
	const {
		renderModal,
		filter,
		setFilter,
		isFilterApplied,
		options,
		autocompleteOptions,
		defaultValue,
		areValuesEquals,
		isApplyButtonDisabled,
		allowEmptySelection,
		disabledAllSelection = false,
		modalContainerSelector = 'main',
		calculateAvailableMaxHeight = false,
		displayFilterButton = true,
		...restProps
	} = props;
	const theme = useTheme();
	const [isModalShown, setIsModalShown] = useState(false);
	const [containerMaxHeight, setContainerMaxHeight] = useState<
		number | undefined
	>();

	const handleSetAvailableHeight = useMemo(() => {
		if (calculateAvailableMaxHeight) {
			return (availableHeight: number) =>
				setContainerMaxHeight(availableHeight - MODAL_AVAILABLE_HEIGHT_OFFSET);
		}

		return undefined;
	}, [calculateAvailableMaxHeight]);

	const [, startTransition] = useTransition();

	const [selection, setSelection] = useState<FilterType>(filter);
	const [prevSelection, setPrevSelection] = useState(filter);

	const filterContainerRef = useRef<HTMLButtonElement>(null);
	const modalRef = useRef<HTMLDivElement>(null);

	useEffect(() => {
		if (!isModalShown) {
			setSelection(filter);
			setPrevSelection(filter);
		}
	}, [filter, isModalShown]);

	const closeModal = () => {
		setIsModalShown(false);
		setSelection(defaultValue);
	};

	const handleReset = () => {
		closeModal();

		startTransition(() => {
			setFilter(defaultValue);
			setPrevSelection(defaultValue);
		});
	};

	const handleApply = () => {
		closeModal();

		startTransition(() => {
			setFilter(selection);
			setPrevSelection(selection);
		});
	};

	const handleClickOutside = useCallback(() => {
		setIsModalShown(false);
		setSelection(prevSelection);
	}, [prevSelection]);

	useOutsideClickMany(
		[modalRef, filterContainerRef as RefObject<HTMLElement>],
		handleClickOutside
	);

	useEffect(() => {
		const handleSubmit = (e: KeyboardEvent) => {
			if (isModalShown) {
				if (e.key === EVENT_ENTER_KEY) {
					handleApply();
				}
			}
		};

		document.addEventListener('keydown', handleSubmit);

		return () => document.removeEventListener('keydown', handleSubmit);
	}, [isModalShown]);

	useLayoutEffect(() => {
		if (!displayFilterButton) {
			setIsModalShown(false);
		}
	}, [displayFilterButton]);

	const isResetDisabled = useMemo(
		() =>
			areValuesEquals(defaultValue, prevSelection) &&
			areValuesEquals(prevSelection, selection),
		[defaultValue, prevSelection, selection]
	);

	const isApplyDisabled = useMemo(
		() =>
			isApplyButtonDisabled
				? isApplyButtonDisabled(prevSelection, selection)
				: areValuesEquals(selection, prevSelection),
		[selection, prevSelection, isApplyButtonDisabled]
	);

	const handleFilterControlButtonClick = (e: MouseEvent<HTMLButtonElement>) => {
		e.stopPropagation();

		setIsModalShown((prevModalShown) => {
			if (prevModalShown) {
				setSelection(prevSelection);
			}

			return !prevModalShown;
		});
	};

	const disableHeaderCellPropagation = (e: SyntheticEvent) => {
		e.stopPropagation();
	};

	const handleKeyDown = (e: React.KeyboardEvent) => {
		e.stopPropagation();

		if (isModalShown) {
			if (e.key === EVENT_ENTER_KEY) {
				handleApply();
			}
		}
	};

	const defaultHeaderCell = headerCellRenderer(restProps);

	if (!renderModal) {
		return null;
	}

	return (
		<HeaderCellWithFilterContentWrapper>
			{defaultHeaderCell}
			<FilterControlWrapper
				onKeyDown={handleKeyDown}
				onClick={disableHeaderCellPropagation}
			>
				{displayFilterButton && (
					<FilterButton
						onClick={handleFilterControlButtonClick}
						isActive={isFilterApplied!}
						isOpen={isModalShown}
						ref={filterContainerRef}
					/>
				)}

				<Popover
					zIndex={zIndexes.filterPopup}
					ref={modalRef}
					open={isModalShown}
					anchorElement={filterContainerRef.current}
					distanceToAnchorElement={8}
					position="bottom-end"
					getPortalContainer={getPortalContainer(modalContainerSelector)}
					withTransition={false}
					fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
					availableHeight={calculateAvailableMaxHeight}
					setAvailableHeight={handleSetAvailableHeight}
					contentWrapperStyles={{
						overflow: 'hidden',
					}}
					wrapperStyles={{
						background: theme.palette['background-white'],
						overflowX: 'hidden',
						borderRadius: BorderRadiuses.m,
						width: MODAL_WIDTH,
						border: `1px solid ${theme.palette['graphics-input-stroke']}`,
						boxShadow: theme.namedShadows.dropDownListWrapper,
					}}
				>
					{renderModal({
						onApply: handleApply,
						onReset: handleReset,
						selection,
						setSelection,
						resetDisabled: isResetDisabled,
						applyDisabled: isApplyDisabled,
						options,
						allowEmptySelection,
						autocompleteOptions,
						disabledAllSelection,
						conteinerMaxHeight: containerMaxHeight,
						isOpen: isModalShown,
					})}
				</Popover>
			</FilterControlWrapper>
		</HeaderCellWithFilterContentWrapper>
	);
}
