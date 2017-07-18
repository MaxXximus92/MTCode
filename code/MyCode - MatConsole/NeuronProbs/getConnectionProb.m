function [conProb,string] = getConnectionProb(net)

types = net.neuronTypes();
weights= net.weightsMatrix();

 esTypes=strcmp(types,'ES');
 emTypes=strcmp(types,'EM');
 isTypes=strcmp(types,'IS');
 imTypes=strcmp(types,'IM');
 dTypes =strcmp(types,'D');
 
 pd_es= getP(dTypes,esTypes);
 pes_is= getP(esTypes,isTypes);
 pis_is= getP(isTypes,isTypes);
 pis_es= getP(isTypes,esTypes);
 pes_em= getP(esTypes,emTypes);
 pem_im= getP(emTypes,imTypes);
 pim_im= getP(imTypes,imTypes);
 pim_em= getP(imTypes,emTypes);
 
    function p =getP(con1,con2)
        c1=sum(sum(weights(con1,con2)~=0));
        c2= sum(con1)*sum(con2);
        p=c1/c2;
    end

conProb = struct('D_ES',pd_es,'ES_IS',pes_is,'IS_IS',pis_is,'IS_ES',pis_es,'ES_EM',pes_em,'EM_IM',pem_im,'IM_IM',pim_im,'IM_EM',pim_em);
end

