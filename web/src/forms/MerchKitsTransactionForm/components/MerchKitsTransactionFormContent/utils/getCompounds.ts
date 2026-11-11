import { CompoundItem } from '@/pages/Merch/components/KitsTab/types';

export const getCompounds = (compounds: CompoundItem[], kitAmount: number) =>
	compounds.map((compound) => ({
		...compound,
		amountInKit: (compound.amountInKit ?? 1) * (kitAmount || 1),
	}));
