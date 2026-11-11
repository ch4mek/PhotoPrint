import { FromToProps } from '@/components/Grid';

export interface ContractsFilters {
	contractFrom: FromToProps<Date>;
	contractTo: FromToProps<Date>;
	fireDate: FromToProps<Date>;
	fireType: string[];
	fireReason: string;
	fireDescription: string;
}

export enum ContractsFilterActionType {
	SET_CONTRACT_FROM_FILTER,
	SET_CONTRACT_TO_FILTER,
	SET_FIRE_DATE_FILTER,
	SET_FIRE_TYPE_FILTER,
	SET_FIRE_REASON_FILTER,
	SET_FIRE_DESCRIPTION_FILTER,
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
			type: ContractsFilterActionType.SET_FIRE_DATE_FILTER;
			payload: FromToProps<Date>;
	  }
	| {
			type: ContractsFilterActionType.SET_FIRE_TYPE_FILTER;
			payload: string[];
	  }
	| {
			type: ContractsFilterActionType.SET_FIRE_REASON_FILTER;
			payload: string;
	  }
	| {
			type: ContractsFilterActionType.SET_FIRE_DESCRIPTION_FILTER;
			payload: string;
	  }
	| {
			type: ContractsFilterActionType.RESET_FILTERS;
	  };

export interface SetFiltersType {
	reset: () => void;
	contractFrom: (payload: FromToProps<Date>) => void;
	contractTo: (payload: FromToProps<Date>) => void;
	fireDate: (payload: FromToProps<Date>) => void;
	fireType: (payload: string[]) => void;
	fireReason: (payload: string) => void;
	fireDescription: (payload: string) => void;
}
