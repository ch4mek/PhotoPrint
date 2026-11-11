export const getMonthColumnWidths = (period: string[], width: number) =>
	period.reduce((acc, month) => {
		acc[month.toLowerCase()] = {
			width,
			fixed: true,
		};

		return acc;
	}, {} as Record<string, { width: number; fixed: boolean }>);
