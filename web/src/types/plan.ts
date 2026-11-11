export interface PlanResponse {
	plan: string | null;
}

export interface PlanFormValues {
	plan: string | null;
}

export interface UpdatePlan extends PlanFormValues {
	userId: number | undefined;
}
