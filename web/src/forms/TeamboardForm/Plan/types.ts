import { PlanFormValues, UpdatePlan } from '@/types/plan';

export interface PlanFormProps {
	handleClose: VoidFunction;
	onSubmit: (values: UpdatePlan) => Promise<object | undefined>;
	userId: number | undefined;
	initialValues: PlanFormValues;
}
