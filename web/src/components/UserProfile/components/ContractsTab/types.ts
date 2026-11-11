import { FromToProps } from '@/components/Grid';

export interface ContractsFilters {
	contractFrom: FromToProps<Date>;
	contractTo: FromToProps<Date>;
}

export type UserContractGridRow = Record<keyof ContractsFilters, string> & {
	id: number | string;
};

export enum ContractsFilterActionType {
	SET_CONTRACT_NUMBER_FILTER,
	SET_CONTRACT_FROM_FILTER,
	SET_CONTRACT_TO_FILTER,
	RESET_FILTERS,
}

export type ContractsFilterAction =
	| {
			type: ContractsFilterActionType.SET_CONTRACT_FROM_FILTER;
			payload: FromToProps<Date>;
	  }
	| {
			type: ContractsFilterActionType.SET_CONTRACT_TO_FILTER;
			payload: FromToProps<Date>;
	  }
	| {
			type: ContractsFilterActionType.RESET_FILTERS;
	  };

export interface SetFiltersType {
	reset: () => void;
	contractFrom: (payload: FromToProps<Date>) => void;
	contractTo: (payload: FromToProps<Date>) => void;
}
