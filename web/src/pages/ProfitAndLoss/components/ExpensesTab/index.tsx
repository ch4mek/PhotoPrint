import { useMemo } from 'react';

import { HeaderInnerBlockWrapper, PageHeaderWrapper } from '../../styles';

import { ButtonAdd } from '@/components/ButtonAdd';
import { DeleteConfirmationDialog } from '@/components/DeleteConfirmationDialog';
import { Loader } from '@/components/Loader';
import { ExpenseForm } from '@/forms/ExpenseForm';
import { EXPENSE_CATEGORY_ENTITY_NAME } from '@/forms/ExpenseForm/constants';
import { createGridPagination } from '@/helpers/createGridPagination';
import { useTableState } from '@/pages/hooks/useTableState';
import { PageWrapper } from '@/pages/styles';
import { useGetExpenses } from '@/queries/expenses/useGetExpenses';

import { ExpensesGrid } from './components/ExpensesGrid';
import { YearFilterControls } from './components/YearFilterControls';
import { expensesGridColumns } from './constants';
import { ExpensesTabProvider, useExpensesTabContext } from './context';
import { useDeleteExpenseCategoryForm } from './hooks/useDeleteExpenseCategoryForm';
import { useExpenseCategoryForm } from './hooks/useExpenseCategoryForm';
import { useExpensesFilterOptions } from './hooks/useExpensesFilterOptions';
import { useExpensesFilters } from './hooks/useExpensesFilters';
import { formatRows } from './utils/formatRows';

function ExpensesTabContent() {
	const { year, setYear, resetYear, isResetDisabled } = useExpensesTabContext();
	const { filters, setFilters, isResetFiltersDisabled } = useExpensesFilters();
	const { options: filterOptions } = useExpensesFilterOptions();

	const {
		sortColumns,
		rowsPerPage,
		page,
		setPage,
		order,
		orderBy,
		handleSortColumnChange,
		handleOnPerPageChange,
	} = useTableState({
		resetPageDeps: [filters, year],
		initialOrderBy: expensesGridColumns.expense.key,
	});

	const { data: expensesData, isFetching } = useGetExpenses(
		Number(rowsPerPage),
		page,
		order,
		orderBy,
		year,
		filters
	);

	const rows = useMemo(
		() => formatRows(expensesData?.categoriesExpenses?.items ?? []),
		[expensesData?.categoriesExpenses?.items]
	);
	const summary = useMemo(
		() => expensesData?.expensesSumsByMonth ?? [],
		[expensesData?.expensesSumsByMonth]
	);

	const {
		expenseId,
		isExpenseFormOpen,
		handleOpenExpenseForm,
		handleCloseExpenseForm,
		expenseEditingInitialValues,
		handleSubmitExpenseForm,
		handleEdit,
	} = useExpenseCategoryForm({ rows });

	const {
		isDeleteDialogOpen,
		expenseToDelete,
		handleCloseDeleteDialog,
		handleSubmitDeleteDialog,
		isDeleteExpenseCategoryLoading,
		handleDelete,
	} = useDeleteExpenseCategoryForm();

	const paginationProps = createGridPagination(
		rows.length,
		page,
		setPage,
		Number(rowsPerPage),
		handleOnPerPageChange,
		expensesData?.categoriesExpenses?.total ?? 0
	);

	return (
		<PageWrapper>
			<PageHeaderWrapper>
				<YearFilterControls
					onYearChange={setYear}
					onReset={resetYear}
					{...{ year, isResetDisabled }}
				/>

				<HeaderInnerBlockWrapper>
					<ButtonAdd onAdd={handleOpenExpenseForm} disabled={isFetching} />
				</HeaderInnerBlockWrapper>
			</PageHeaderWrapper>

			<Loader isLoading={isFetching}>
				<ExpensesGrid
					{...{
						rows,
						summary,
						filters,
						setFilters,
						isResetFiltersDisabled,
						sortColumns,
						handleSortColumnChange,
						paginationProps,
						handleEdit,
						handleDelete,
						filterOptions,
					}}
				/>
			</Loader>

			{isExpenseFormOpen && (
				<ExpenseForm
					expenseId={expenseId}
					initialValues={expenseEditingInitialValues}
					onClose={handleCloseExpenseForm}
					onSubmit={handleSubmitExpenseForm}
				/>
			)}

			{isDeleteDialogOpen && expenseToDelete && (
				<DeleteConfirmationDialog
					title={EXPENSE_CATEGORY_ENTITY_NAME}
					item={expenseToDelete.expense}
					handleSubmit={handleSubmitDeleteDialog}
					handleClose={handleCloseDeleteDialog}
					isLoading={isDeleteExpenseCategoryLoading}
				/>
			)}
		</PageWrapper>
	);
}

export function ExpensesTab() {
	return (
		<ExpensesTabProvider>
			<ExpensesTabContent />
		</ExpensesTabProvider>
	);
}
