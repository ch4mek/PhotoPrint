import { FromToProps } from '@/components/Grid';
import {
	PerformanceReviewFilter,
	PerformanceReviewTableFiltersKeys,
} from '@/pages/TeamBoardUser/types';

interface Action<T = undefined> {
	type: PerformanceReviewTableFiltersKeys;
	payload: T;
}

interface SetPerformanceDateReviewAction extends Action<FromToProps<Date>> {
	type: PerformanceReviewTableFiltersKeys.performanceReviewDate;
}

interface SetAgreementsAction extends Action<string> {
	type: PerformanceReviewTableFiltersKeys.agreements;
}

interface SetEmployeeThesesAction extends Action<string> {
	type: PerformanceReviewTableFiltersKeys.employeeTheses;
}

interface SetThesesAction extends Action<string> {
	type: PerformanceReviewTableFiltersKeys.theses;
}

interface SetAction extends Action<PerformanceReviewFilter> {
	type: PerformanceReviewTableFiltersKeys.set;
}

export type PerformanceReviewAction =
	| SetPerformanceDateReviewAction
	| SetAgreementsAction
	| SetEmployeeThesesAction
	| SetThesesAction
	| SetAction;
