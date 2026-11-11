import { FilledEditingButton } from '@mms/mms-ui-library/components/FilledEditingButton';
import { useCallback, MouseEvent, useRef, useEffect, useState } from 'react';
import type { KeyboardEvent as ReactKeyboardEvent } from 'react';
import { createPortal } from 'react-dom';

import { getPortalContainer } from '@/components/Grid/utils/getPortalContainer';
import { useExpensesTabContext } from '@/pages/ProfitAndLoss/components/ExpensesTab/context';
import { PRICE_DECIMAL_DIVIDER } from '@/pages/ProfitAndLoss/constants';
import { KeyboardKey } from '@/types/keyboard';

import {
	EXPENSE_CELL_HEIGHT,
	MASK_MAX,
	MASK_MIN,
	MASK_SCALE,
} from './constants';
import { ExpenseAmountDisplay } from './ExpenseAmountDisplay';
import { useRowUpdater } from './hooks/useRowUpdater';
import {
	ExpenseAmountInput,
	ExpenseAmountInputWrapper,
	ExpenseCellActions,
	ExpenseCellInteractive,
	ExpenseCellWrapper,
} from './styles';
import { EditableExpenseCellProps } from './types';

export function EditableCellRenderer({
	expenseCategoryId,
	expenseId,
	value,
	initialValue,
	month,
}: EditableExpenseCellProps) {
	const { year, saveExpense, isSavingExpense } = useExpensesTabContext();
	const isPending = isSavingExpense(expenseCategoryId, month);
	const cellRef = useRef<HTMLDivElement>(null);
	const [cellRect, setCellRect] = useState<DOMRect | null>(null);

	const {
		inputState,
		inputRef,
		setCellValue,
		setIsInputActive,
		handleCellValueAccept,
		acceptChanges,
		handleUpdateError,
	} = useRowUpdater(value);

	const handleCellClick = useCallback(() => {
		if (!isPending) {
			setCellValue(value);
			setIsInputActive(true);
		}
	}, [isPending, setIsInputActive, setCellValue, value]);

	const handleSave = useCallback(
		async (event?: MouseEvent) => {
			event?.stopPropagation();

			if (+inputState.cellValue === initialValue) {
				setIsInputActive(false);

				return;
			}

			const newValue = +inputState.cellValue;

			await saveExpense(
				{
					expenseId,
					expenseCategoryId,
					year: Number(year),
					monthOfYear: month + 1,
					expenseAmount: newValue,
					previousAmount: initialValue ?? 0,
				},
				{ onError: handleUpdateError(initialValue ?? 0) }
			);
			acceptChanges(newValue);
		},
		[
			inputState.cellValue,
			initialValue,
			saveExpense,
			acceptChanges,
			handleUpdateError,
			expenseId,
			expenseCategoryId,
			year,
			month,
			setIsInputActive,
		]
	);

	const handleInputKeyDown = useCallback(
		async (event: ReactKeyboardEvent<HTMLInputElement>) => {
			if (!inputState.isActive) {
				return;
			}

			if (
				event.key === KeyboardKey.ArrowLeft ||
				event.key === KeyboardKey.ArrowRight
			) {
				event.stopPropagation();

				return;
			}

			if (event.key !== KeyboardKey.Tab && event.key !== KeyboardKey.Enter) {
				return;
			}

			event.preventDefault();
			event.stopPropagation();

			await handleSave();
		},
		[inputState.isActive, handleSave]
	);

	const handleBlur = useCallback(() => {
		setCellValue(value);
		setIsInputActive(false);
	}, [value, setCellValue, setIsInputActive]);

	useEffect(() => {
		if (!inputState.isActive) {
			setCellRect(null);

			return;
		}

		if (cellRef.current) {
			setCellRect(cellRef.current.getBoundingClientRect());
		}

		const handleScrollOrResize = () => {
			setCellRect(null);
			handleBlur();
		};

		window.addEventListener('scroll', handleScrollOrResize, true);
		window.addEventListener('resize', handleScrollOrResize);

		return () => {
			window.removeEventListener('scroll', handleScrollOrResize, true);
			window.removeEventListener('resize', handleScrollOrResize);
		};
	}, [inputState.isActive, handleBlur]);

	const portalContainer = getPortalContainer('[modal-container]')();

	const actionsPortal = cellRect
		? createPortal(
				<ExpenseCellActions
					style={{
						left: cellRect.left,
						top: cellRect.bottom + 4,
						width: cellRect.width,
					}}
				>
					<FilledEditingButton variant="cancel" onMouseDown={handleBlur} />
					<FilledEditingButton variant="apply" onMouseDown={handleSave} />
				</ExpenseCellActions>,
				portalContainer
		  )
		: null;

	const expenseCellContent = !inputState.isActive ? (
		<ExpenseAmountDisplay value={value} />
	) : (
		<ExpenseAmountInputWrapper>
			<ExpenseAmountInput
				mask={Number}
				scale={MASK_SCALE}
				radix={PRICE_DECIMAL_DIVIDER}
				mapToRadix={[PRICE_DECIMAL_DIVIDER]}
				thousandsSeparator=""
				normalizeZeros={false}
				padFractionalZeros={false}
				signed={false}
				value={inputState.cellValue}
				onAccept={handleCellValueAccept}
				onBlur={handleBlur}
				onKeyDown={handleInputKeyDown}
				inputRef={inputRef}
				min={MASK_MIN}
				max={MASK_MAX}
			/>
			{actionsPortal}
		</ExpenseAmountInputWrapper>
	);

	return (
		<ExpenseCellWrapper ref={cellRef}>
			<ExpenseCellInteractive
				data-clickable="true"
				backgroundColor="inherit"
				onClick={handleCellClick}
				isDisabled={isPending}
				minHeight={EXPENSE_CELL_HEIGHT}
				isClickable
			>
				{expenseCellContent}
			</ExpenseCellInteractive>
		</ExpenseCellWrapper>
	);
}
