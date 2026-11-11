import { useCallback, useMemo } from 'react';

import { ListLabel } from '@/forms/Absence/AbsenceAssignmentForm/components/TransferList/components/ListLabel';
import { TransferListItem } from '@/forms/Absence/AbsenceAssignmentForm/types';
import { sortById } from '@/helpers/sortById';

import { List } from './components/List';
import { TransferControls } from './components/TransferControls';
import { ListContentWrapper, TransferListWrapper } from './styles';
import { ListLabelType, TransferListProps } from './types';
import { differenceById } from './utils/differenceById';
import { filterItems } from './utils/filterItems';
import { intersectById } from './utils/intersectById';

export function TransferList({
	unselectedItems,
	selectedItems,
	setSelectedItems,
	setUnselectedItems,
	searchValue,
	checkedItems,
	setCheckedItems,
}: TransferListProps) {
	const filteredUnselected = useMemo(
		() => filterItems(searchValue, unselectedItems),
		[unselectedItems, searchValue]
	);
	const filteredSelected = useMemo(
		() => filterItems(searchValue, selectedItems),
		[selectedItems, searchValue]
	);

	const leftChecked = useMemo(
		() => intersectById(checkedItems, unselectedItems),
		[checkedItems, unselectedItems]
	);
	const rightChecked = useMemo(
		() => intersectById(checkedItems, selectedItems),
		[checkedItems, selectedItems]
	);

	const handleToggle = useCallback(
		(value: TransferListItem) => () => {
			const exists = checkedItems.find((item) => item.id === value.id);
			const newChecked = exists
				? checkedItems.filter((item) => item.id !== value.id)
				: [...checkedItems, value];

			setCheckedItems(newChecked);
		},
		[checkedItems]
	);

	const checkIsChecked = useCallback(
		(id: number) => checkedItems.some((item) => item.id === id),
		[checkedItems]
	);

	const handleAllRight = useCallback(() => {
		setSelectedItems(sortById([...selectedItems, ...unselectedItems]));
		setCheckedItems([
			...checkedItems,
			...differenceById(unselectedItems, checkedItems),
		]);
		setUnselectedItems([]);
	}, [selectedItems, unselectedItems, checkedItems]);

	const handleCheckedRight = useCallback(() => {
		setSelectedItems(sortById([...selectedItems, ...leftChecked]));
		setUnselectedItems(differenceById(unselectedItems, leftChecked));
	}, [selectedItems, leftChecked, unselectedItems]);

	const handleCheckedLeft = useCallback(() => {
		setSelectedItems(differenceById(selectedItems, rightChecked));
		setUnselectedItems(sortById([...unselectedItems, ...rightChecked]));
	}, [selectedItems, rightChecked, unselectedItems]);

	const handleAllLeft = useCallback(() => {
		setSelectedItems([]);
		setCheckedItems([
			...checkedItems,
			...differenceById(selectedItems, checkedItems),
		]);
		setUnselectedItems(sortById([...unselectedItems, ...selectedItems]));
	}, [selectedItems, unselectedItems, checkedItems]);

	return (
		<TransferListWrapper>
			<ListContentWrapper>
				<ListLabel
					count={filteredUnselected.length}
					label={ListLabelType.Unassigned}
				/>
				<List
					items={filteredUnselected}
					checkIsChecked={checkIsChecked}
					handleCheckboxClick={handleToggle}
				/>
			</ListContentWrapper>
			<TransferControls
				{...{
					handleAllRight,
					handleCheckedRight,
					handleCheckedLeft,
					handleAllLeft,
				}}
				isAllRightDisabled={unselectedItems.length === 0}
				isCheckedRight={leftChecked.length === 0}
				isCheckedLeft={rightChecked.length === 0}
				isAllLeft={selectedItems.length === 0}
			/>
			<ListContentWrapper>
				<ListLabel
					count={filteredSelected.length}
					label={ListLabelType.Assigned}
				/>
				<List
					items={filteredSelected}
					checkIsChecked={checkIsChecked}
					handleCheckboxClick={handleToggle}
				/>
			</ListContentWrapper>
		</TransferListWrapper>
	);
}
